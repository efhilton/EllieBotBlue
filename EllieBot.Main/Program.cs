﻿using EllieBot.Ambulator;
using EllieBot.Brain;
using Newtonsoft.Json;
using System;
using System.IO.Abstractions;

namespace EllieBot
{
    internal class Program
    {
        private static readonly string DEFAULT_CONFIG_FILE_NAME = "robot_config.json";

        public static void Main(string[] args)
        {
            IFileSystem fileSystem = new FileSystem();
            RobotConfig configs = RobotConfig.LoadFile(DEFAULT_CONFIG_FILE_NAME, fileSystem, Program.Logger).GetAwaiter().GetResult();

            IMotor motorLeft = new RealMotor("Left", configs.LeftMotorForwardPin, configs.LeftMotorBackwardPin, Program.Logger);
            IMotor motorRight = new RealMotor("Right", configs.RightMotorForwardPin, configs.RightMotorBackwardPin, Program.Logger);
            IMotorsController motors = new MotorsController(motorLeft, motorRight, Program.Logger);
            motors.Initialize();

            ICommandProcessor proc = new CommandProcessor(motors);

            Robot p = new Robot(proc, motors, configs);

            p.Initialize().Wait();

            RobotCommand rc = new RobotCommand
            {
                Command = "go",
                Arguments = new string[] { "-0.02", "0.99" }
            };

            string json = JsonConvert.SerializeObject(rc);
            p.PublishAsync(json).Wait();

            Console.WriteLine("Robot is Running. Press any key to Exit");
            Console.ReadLine();
        }

        private static void Logger(string msg)
        {
            Console.Out.WriteLine(msg);
        }
    }
}