using EllieBot.Ambulator;
using EllieBot.Brain;
using Newtonsoft.Json;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace EllieBot {

    internal class Program {
        private static readonly string DEFAULT_CONFIG_FILE_NAME = "robot_config.json";

        public static void Main(string[] args) {
            IFileSystem fileSystem = new FileSystem();
            RobotConfig configs = RobotConfig.LoadFile(DEFAULT_CONFIG_FILE_NAME, fileSystem, Logger).GetAwaiter().GetResult();

            IMotor motorLeft = new RealMotor("Left", configs.LeftMotorForwardPin, configs.LeftMotorBackwardPin, Logger);
            IMotor motorRight = new RealMotor("Right", configs.RightMotorForwardPin, configs.RightMotorBackwardPin, Logger);
            IMotorsController motors = new RawMotorsController(motorLeft, motorRight, Logger);
            motors.Initialize();

            ICommandProcessor proc = new CommandProcessor();

            Robot p = new Robot(proc, motors, configs);

            p.Initialize().Wait();
            SendStopCommand(p).Wait();

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
