using EllieBot.Brain;
using EllieBot.Brain.Commands;
using EllieBot.IO;
using EllieBot.IO.Devices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace EllieBot {

    internal class Program {

        public static void Main(string[] args) {
            Action<string> logger = Logger;

            GpioController controller;
            try {
                controller = new GpioController();
                logger?.Invoke("IO Enabled.");
            } catch (NotSupportedException) {
                controller = null;
                logger?.Invoke("No GPIO. IO Disabled");
            }

            IFileSystem fileSystem = new FileSystem();
            RobotConfig configs = RobotConfig.LoadFile(Defaults.Internal.CONFIG_FILE_NAME, fileSystem, logger).GetAwaiter().GetResult();

            ICommandProcessor commandProcessor = new CommandProcessor(logger);

            if (configs.HBridgeMotorDescriptions != null && configs.HBridgeMotorDescriptions.Length > 0) {
                List<IPWMDevice> motors = new List<IPWMDevice>();
                foreach (HBridgeMotorDescription h in configs.HBridgeMotorDescriptions) {
                    IPWMDevice motor = new HBridgeMotor(h.UniqueId, h.ForwardPin, h.BackwardPin, logger);
                    motors.Add(motor);
                }
                IPWMController pwmController = PwmController.CreateInstance(controller, motors, logger);
                commandProcessor.RegisterCommand(new DriveMotorControl(configs.LeftMotorUniqueId, configs.RightMotorUniqueId, pwmController, logger));
                commandProcessor.RegisterCommand(new SetPwmControl(pwmController, logger));
            }

            if (configs.LedDescriptions != null && configs.LedDescriptions.Length > 0) {
                List<IBlinkable> leds = new List<IBlinkable>();
                foreach (LedDescription l in configs.LedDescriptions) {
                    LED led = new LED(l.UniqueId, l.PinNumber, logger);
                    leds.Add(led);
                }
                commandProcessor.RegisterCommand(new SetLedControl(leds, logger));
            }

            Robot robot = Robot.CreateInstance(commandProcessor, configs, logger);
            robot.Initialize().Wait();
            SendStopCommand(robot).Wait();

            Console.WriteLine("Robot is Running. Press any key to Exit");
            Console.ReadLine();
        }

        private static Task SendStopCommand(Robot p) {
            CommandPacket rc = new CommandPacket {
                Command = "go.tank",
                Arguments = new string[] { "0.0", "0.0" }
            };

            string json = JsonConvert.SerializeObject(rc);
            return p.PublishAsync(json);
        }

        private static void Logger(string msg) {
            Console.Out.WriteLine(msg);
        }
    }
}
