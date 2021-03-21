using EllieBot.Ambulator;
using EllieBot.Brain;
using Newtonsoft.Json;
using System;

namespace EllieBot
{
    class Program
    {
        public static void Main(string[] args)
        {
            IMotors motors = new DummyMotors();
            ICommandProcessor proc = new CommandProcessor(motors);

            Robot p = new Robot(proc);
            p.Initialize().Wait();

            RobotCommand rc = new RobotCommand
            {
                Command = "go",
                Arguments = new string[] { "-0.1", "0.2" }
            };
            string json = JsonConvert.SerializeObject(rc);
            p.PublishAsync(json).Wait();
            Console.ReadLine();
        }
    }
}
