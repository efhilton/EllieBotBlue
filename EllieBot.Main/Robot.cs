using EllieBot.Brain;
using EllieBot.Brain.Commands;
using EllieBot.Configs;
using EllieBot.Configs.Descriptions;
using EllieBot.IO;
using EllieBot.IO.Devices;
using EllieBot.Logging;
using EllieBot.NervousSystem;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Text;
using System.Threading.Tasks;

namespace EllieBot {

    public class Robot {
        private Communications.NervousSystem comms;
        private readonly ICommandProcessor CommandProcessor;
        private readonly RobotConfig Configuration;
        private readonly ILogger Logger;
        private readonly GpioController GpioController;
        public static Robot Instance { get; private set; }

        private Robot(ICommandProcessor cmdProcessor, GpioController controller, RobotConfig configs, ILogger logger) {
            this.CommandProcessor = cmdProcessor;
            this.Configuration = configs;
            this.Logger = logger;
            this.GpioController = controller;
        }

        public Task Initialize() {
            this.CommandProcessor.Initialize();
            this.comms = new Communications.NervousSystem();
            this.comms.ConnectAsync(this.Configuration.MqttConnectionDescription.Host, this.Configuration.MqttConnectionDescription.Port).Wait();

            this.Logger.LogMessageFcn = this.PublishLogAsync;

            Task t1 = RegisterHBridgeMotors(Logger, GpioController, Configuration, CommandProcessor);
            Task t2 = RegisterOutputPins(Logger, GpioController, Configuration, CommandProcessor);
            Task t3 = this.RegisterSensors(Logger, GpioController, Configuration);
            Task t4 = this.comms.SubscribeAsync(this.Configuration.MqttConnectionDescription.TopicForCommands,
                                                this.OnCommandReceived,
                                                this.OnCommandConnection,
                                                this.OnCommandDisconnection);
            return Task.WhenAll(t1, t2, t3, t4);
        }

        internal static Robot CreateInstance(GpioController gpioController, RobotConfig configs, ILogger logger) {
            if (Instance != null) {
                return Instance;
            }

            ICommandProcessor commandProcessor = new CommandProcessor(logger);
            Instance = new Robot(commandProcessor, gpioController, configs, logger);
            return Instance;
        }

        public Task PublishCommandAsync(string message)
            => message is null
            ? throw new ArgumentNullException(message)
            : this.comms.PublishAsync(this.Configuration.MqttConnectionDescription.TopicForCommands, message, false, 0);

        public Task PublishLogAsync(Constants.LoggingLevel level, string message) {
            LoggingPdu pdu = new LoggingPdu {
                Level = level,
                Message = message.Trim()
            };
            string json = JsonConvert.SerializeObject(pdu);
            return this.comms.PublishAsync(this.Configuration.MqttConnectionDescription.TopicForLogging, json, false, 0);
        }

        public Task PublishDataAsync(string json) =>
             json is null
                ? throw new ArgumentNullException(nameof(json))
                : this.comms.PublishAsync(this.Configuration.MqttConnectionDescription.TopicForSensorData, json, false, 0);

        private Task OnCommandDisconnection(MqttClientDisconnectedEventArgs arg) => Task.Run(() => this.Logger.Info("Disconnected. Until next time!"));

        private Task OnCommandConnection(MqttClientConnectedEventArgs arg) => Task.Run(() => this.Logger.Info("Connected! Ready for commands!"));

        private Task OnCommandReceived(MqttApplicationMessageReceivedEventArgs arg) {
            return Task.Run(() => {
                string Payload = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                try {
                    CommandPdu cmd = JsonConvert.DeserializeObject<CommandPdu>(Payload);
                    this.CommandProcessor.QueueExecute(cmd);
                } catch (Exception) {
                    this.Logger.Debug($"Ignored: {Payload}");
                }
            });
        }

        private static Task RegisterHBridgeMotors(ILogger logger, GpioController controller, RobotConfig configs, ICommandProcessor commandProcessor) {
            return Task.Run(() => {
                if (configs.Actuators != null && configs.Actuators.HBridgeMotorDescriptions != null && configs.Actuators.HBridgeMotorDescriptions.Length > 0) {
                    List<IPWMDevice> motors = new List<IPWMDevice>();
                    foreach (HBridgeMotorDescription h in configs.Actuators.HBridgeMotorDescriptions) {
                        IPWMDevice motor = new HBridgeMotor(h.UniqueId.ToLower(), h.ForwardPinNumber, h.BackwardPinNumber, logger);
                        motors.Add(motor);
                    }
                    IPWMController pwmController = PwmController.CreateInstance(controller, motors, logger);
                    commandProcessor.RegisterCommand(new DriveMotorControl(configs.DriveTrainDescription.LeftMotorUniqueId.ToLower(),
                                                                           configs.DriveTrainDescription.RightMotorUniqueId.ToLower(),
                                                                           pwmController));
                    commandProcessor.RegisterCommand(new SetHbridgePwmControl(pwmController));
                }
            });
        }

        private static Task RegisterOutputPins(ILogger logger, GpioController controller, RobotConfig configs, ICommandProcessor commandProcessor) {
            return Task.Run(() => {
                if (configs.Actuators != null && configs.Actuators.OutputPinDescriptions != null && configs.Actuators.OutputPinDescriptions.Length > 0) {
                    List<IBlinkable> leds = new List<IBlinkable>();
                    foreach (OutputPinDescription l in configs.Actuators.OutputPinDescriptions) {
                        LED led = new LED(l.UniqueId.ToLower(), l.OutputPinNumber, logger);
                        leds.Add(led);
                    }
                    ILedController ledController = LedController.CreateInstance(controller, leds, logger);
                    commandProcessor.RegisterCommand(new SetLedControl(ledController));
                }
            });
        }

        private Task RegisterSensors(ILogger logger, GpioController controller, RobotConfig configs) {
            return Task.Run(() => {
                if (configs.Sensors != null) {
                    List<Task> sensors = new List<Task>();
                    if (configs.Sensors.Hcsr04sDescriptions != null && configs.Sensors.Hcsr04sDescriptions.Length > 0) {
                        foreach (Hcsr04sDescription l in configs.Sensors.Hcsr04sDescriptions) {
                            IBlinkable triggerPin = new LED(l.UniqueId.ToLower() + ".trigger", l.TriggerPinNumber, logger);
                            ISensor<PinValue> echoPin = new InputPinTrigger(l.UniqueId.ToLower() + ".echo", l.EchoPinNumber, logger);
                            UltrasonicHCSR04 distanceSensor = new UltrasonicHCSR04(l.UniqueId.ToLower(), triggerPin, echoPin, logger);

                            distanceSensor.OnData += this.OnSingleDoubleData;

                            Task t = distanceSensor.Initialize(controller);

                            sensors.Add(t);

                            this.Logger.Info($"Registered Sensor: {l.UniqueId}");
                        }
                    }

                    if (configs.Sensors.InputPinTriggerDescriptions != null && configs.Sensors.InputPinTriggerDescriptions.Length > 0) {
                        foreach (InputPinTriggerDescription l in configs.Sensors.InputPinTriggerDescriptions) {
                            ISensor<PinValue> inputPin = new InputPinTrigger(l.UniqueId.ToLower(), l.InputPinNumber, logger);

                            inputPin.OnData += this.OnPinValueChanged;

                            Task t = inputPin.Initialize(controller);

                            sensors.Add(t);

                            this.Logger.Info($"Registered Sensor: {l.UniqueId}");
                        }
                    }

                    if (sensors.Count > 0) {
                        Task.WhenAll(sensors.ToArray()).Wait();
                    }
                }
            });
        }

        private void OnPinValueChanged(string id, PinValue data) {
            SensorDataPdu<bool> pdu = new SensorDataPdu<bool> {
                UniqueId = id,
                Data = new bool[] { data == PinValue.High }
            };

            string json = JsonConvert.SerializeObject(pdu);
            this.PublishDataAsync(json).Wait();
        }

        private void OnSingleDoubleData(string id, double data)
            => this.OnDoubleData(id, new double[] { data });

        private void OnDoubleData(string id, double[] data) {
            SensorDataPdu<double> pdu = new SensorDataPdu<double> {
                UniqueId = id,
                Data = data
            };

            string json = JsonConvert.SerializeObject(pdu);
            this.PublishDataAsync(json).Wait();
        }
    }
}
