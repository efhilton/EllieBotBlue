using EllieBot.Brain;
using EllieBot.Brain.Commands;
using EllieBot.Configs;
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
            this.comms.ConnectAsync(this.Configuration.MqttDefinitions.Host, this.Configuration.MqttDefinitions.Port).Wait();

            this.Logger.LogMessageFcn = this.PublishLogAsync;

            Task t1 = RegisterHBridgeMotors(Logger, GpioController, Configuration, CommandProcessor);
            Task t2 = RegisterLEDs(Logger, GpioController, Configuration, CommandProcessor);
            Task t3 = this.RegisterUltrasonicSensors(Logger, GpioController, Configuration);
            Task t4 = this.comms.SubscribeAsync(this.Configuration.MqttDefinitions.TopicForCommands,
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
            : this.comms.PublishAsync(this.Configuration.MqttDefinitions.TopicForCommands, message, false, 0);

        public Task PublishLogAsync(Constants.LoggingLevel level, string message) {
            LoggingPdu pdu = new LoggingPdu {
                Level = level,
                Message = message.Trim()
            };
            string json = JsonConvert.SerializeObject(pdu);
            return this.comms.PublishAsync(this.Configuration.MqttDefinitions.TopicForLogging, json, false, 0);
        }

        public Task PublishDataAsync(string json) =>
             json is null
                ? throw new ArgumentNullException(nameof(json))
                : this.comms.PublishAsync(this.Configuration.MqttDefinitions.TopicForSensorData, json, false, 0);

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
                if (configs.HBridgeMotorDefinitions != null && configs.HBridgeMotorDefinitions.Length > 0) {
                    List<IPWMDevice> motors = new List<IPWMDevice>();
                    foreach (HBridgeMotorDescription h in configs.HBridgeMotorDefinitions) {
                        IPWMDevice motor = new HBridgeMotor(h.UniqueId.ToLower(), h.ForwardPin, h.BackwardPin, logger);
                        motors.Add(motor);
                    }
                    IPWMController pwmController = PwmController.CreateInstance(controller, motors, logger);
                    commandProcessor.RegisterCommand(new DriveMotorControl(configs.DriveTrainDefinitions.LeftMotorUniqueId.ToLower(),
                                                                           configs.DriveTrainDefinitions.RightMotorUniqueId.ToLower(),
                                                                           pwmController));
                    commandProcessor.RegisterCommand(new SetHbridgePwmControl(pwmController));
                }
            });
        }

        private static Task RegisterLEDs(ILogger logger, GpioController controller, RobotConfig configs, ICommandProcessor commandProcessor) {
            return Task.Run(() => {
                if (configs.LedDefinitions != null && configs.LedDefinitions.Length > 0) {
                    List<IBlinkable> leds = new List<IBlinkable>();
                    foreach (LedDescription l in configs.LedDefinitions) {
                        LED led = new LED(l.UniqueId.ToLower(), l.PinNumber, logger);
                        leds.Add(led);
                    }
                    ILedController ledController = LedController.CreateInstance(controller, leds, logger);
                    commandProcessor.RegisterCommand(new SetLedControl(ledController));
                }
            });
        }

        private Task RegisterUltrasonicSensors(ILogger logger, GpioController controller, RobotConfig configs) {
            return Task.Run(() => {
                if (configs.Hcsr04sDescriptions != null && configs.Hcsr04sDescriptions.Length > 0) {
                    List<Task> sensors = new List<Task>();
                    foreach (Hcsr04sDescription l in configs.Hcsr04sDescriptions) {
                        IBlinkable triggerPin = new LED(l.UniqueId.ToLower() + ".trigger", l.TriggerPin, logger);
                        ISensor<PinValue> echoPin = new InputPinTrigger(l.UniqueId.ToLower() + ".echo", l.EchoPin, logger);
                        UltrasonicHCSR04 distanceSensor = new UltrasonicHCSR04(l.UniqueId.ToLower(), triggerPin, echoPin, logger);

                        distanceSensor.OnData += this.OnSingleDoubleData;
                        Task t = distanceSensor.Initialize(controller);

                        sensors.Add(t);
                    }

                    Task.WhenAll(sensors.ToArray()).Wait();
                }
            });
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
