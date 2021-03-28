using EllieBot.IO;
using EllieBot.Brain;
using Newtonsoft.Json;
using System;
using System.Device.Gpio;
using System.IO.Abstractions;
using System.Threading.Tasks;
using EllieBot.IO.Devices;
using EllieBot.Brain.Commands;

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
            RobotConfig configs = RobotConfig.LoadFile(Constants.DEFAULT_CONFIG_FILE_NAME, fileSystem, logger).GetAwaiter().GetResult();

            IBlinkable headlights = new LED(configs.HeadlightsPin, logger);
            IPWMDevice motorLeft = new RealMotor(Constants.LEFT_MOTOR_ID, configs.LeftMotorForwardPin, configs.LeftMotorBackwardPin, logger);
            IPWMDevice motorRight = new RealMotor(Constants.RIGHT_MOTOR_ID, configs.RightMotorForwardPin, configs.RightMotorBackwardPin, logger);

            IPWMController pwmController = PwmController.CreateInstance(controller, new IPWMDevice[] { motorLeft, motorRight }, logger);

            ICommandProcessor commandProcessor = new CommandProcessor();
            commandProcessor.RegisterCommand(new DriveMotorControl(pwmController));
            commandProcessor.RegisterCommand(new SetPwmControl(pwmController));
            commandProcessor.RegisterCommand(new SetLedControl(new IBlinkable[] { headlights }));

            Robot robot = Robot.CreateInstance(commandProcessor, configs, logger);
            robot.Initialize().Wait();
            SendStopCommand(robot).Wait();

            Console.WriteLine("Robot is Running. Press any key to Exit");
            Console.ReadLine();
        }

        private static Task SendStopCommand(Robot p) {
            RobotCommand rc = new RobotCommand {
                Command = "GO_RAW",
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
