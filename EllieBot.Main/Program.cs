using EllieBot.Brain;
using EllieBot.Brain.Commands;
using EllieBot.Configs;
using EllieBot.IO;
using EllieBot.IO.Devices;
using EllieBot.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.IO.Abstractions;
using System.Threading.Tasks;
using static EllieBot.Constants;

namespace EllieBot {

    internal class Program {

        public static void Main(string[] args) {
            MqttLogger logger = new MqttLogger {
                MinLevel = Constants.LoggingLevel.FINE,
                LogMessageFcn = m => Task.Run(() => Console.Out.WriteLine(m))
            };

            GpioController controller;
            try {
                controller = new GpioController();
                logger.Info("IO Enabled.");
            } catch (NotSupportedException) {
                controller = null;
                logger.Info("No GPIO. IO Disabled");
            }

            IFileSystem fileSystem = new FileSystem();
            RobotConfig configs = RobotConfig.LoadFile(Constants.Internal.CONFIG_FILE_NAME, fileSystem, m => logger.Info(m)).GetAwaiter().GetResult();

            ICommandProcessor commandProcessor = new CommandProcessor(logger);

            Task t1 = RegisterHBridgeMotors(logger, controller, configs, commandProcessor);
            Task t2 = RegisterLEDs(logger, controller, configs, commandProcessor);
            Task.WaitAll(t1, t2);

            Robot robot = Robot.CreateInstance(commandProcessor, configs, logger);

            robot.Initialize().Wait();
            logger.MinLevel = CalculateLoggingLevelFromConfigs(configs, LoggingLevel.INFO);
            logger.LogMessageFcn = robot.PublishLogAsync;

            logger.Info("EllieBot is Fully Operational.");

            Console.WriteLine("EllieBot is Operational and Nominal.");
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        private static Task RegisterHBridgeMotors(MqttLogger logger, GpioController controller, RobotConfig configs, ICommandProcessor commandProcessor) {
            return Task.Run(() => {
                if (configs.HBridgeMotorDefinitions != null && configs.HBridgeMotorDefinitions.Length > 0) {
                    List<IPWMDevice> motors = new List<IPWMDevice>();
                    foreach (HBridgeMotorDescription h in configs.HBridgeMotorDefinitions) {
                        IPWMDevice motor = new HBridgeMotor(h.UniqueId, h.ForwardPin, h.BackwardPin, logger);
                        motors.Add(motor);
                    }
                    IPWMController pwmController = PwmController.CreateInstance(controller, motors, logger);
                    commandProcessor.RegisterCommand(new DriveMotorControl(configs.DriveTrainDefinitions.LeftMotorUniqueId,
                                                                           configs.DriveTrainDefinitions.RightMotorUniqueId,
                                                                           pwmController,
                                                                           logger));
                    commandProcessor.RegisterCommand(new SetPwmControl(pwmController));
                }
            });
        }

        private static Task RegisterLEDs(MqttLogger logger, GpioController controller, RobotConfig configs, ICommandProcessor commandProcessor) {
            return Task.Run(() => {
                if (configs.LedDefinitions != null && configs.LedDefinitions.Length > 0) {
                    List<IBlinkable> leds = new List<IBlinkable>();
                    foreach (LedDescription l in configs.LedDefinitions) {
                        LED led = new LED(l.UniqueId, l.PinNumber, logger);
                        leds.Add(led);
                    }
                    ILedController ledController = LedController.CreateInstance(controller, leds, logger);
                    commandProcessor.RegisterCommand(new SetLedControl(ledController));
                }
            });
        }

        private static LoggingLevel CalculateLoggingLevelFromConfigs(RobotConfig configs, LoggingLevel fallbackLoggingLevel = LoggingLevel.INFO) {
            return configs == null || string.IsNullOrWhiteSpace(configs.DebuggingLevel) || !Enum.TryParse(configs.DebuggingLevel, out LoggingLevel configLoggingLevel)
                ? fallbackLoggingLevel
                : configLoggingLevel;
        }

        private static Task SendStopCommand(Robot p) {
            CommandPacket rc = new CommandPacket {
                Command = "go.tank",
                Arguments = new string[] { "0.0", "0.0" }
            };

            string json = JsonConvert.SerializeObject(rc);
            return p.PublishCommandAsync(json);
        }
    }
}
