using EllieBot.Configs;
using EllieBot.Logging;
using System;
using System.Device.Gpio;
using System.IO.Abstractions;
using static EllieBot.Constants;

namespace EllieBot {

    internal class Program {

        public static void Main() {
            ILogger logger = new MqttLogger();

            GpioController gpioController;
            try {
                gpioController = new GpioController();
                logger.Warn("IO Enabled.");
            } catch (NotSupportedException) {
                gpioController = null;
                logger.Warn("No GPIO. IO Disabled");
            }

            IFileSystem fileSystem = new FileSystem();
            RobotConfig configs = RobotConfig.LoadFile(Constants.Internal.CONFIG_FILE_NAME, fileSystem, m => logger.Info(m)).GetAwaiter().GetResult();

            logger.MinLevel = CalculateLoggingLevelFromConfigs(configs, LoggingLevel.INFO);
            logger.Info($"Set logging level to {logger.MinLevel}");

            Robot robot = Robot.CreateInstance(gpioController, configs, logger);
            robot.Initialize().Wait();

            logger.Info("EllieBot is Fully Operational.");

            Console.WriteLine("EllieBot is Operational and Nominal.");
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        private static LoggingLevel CalculateLoggingLevelFromConfigs(RobotConfig configs, LoggingLevel fallbackLoggingLevel = LoggingLevel.INFO) {
            return configs == null || string.IsNullOrWhiteSpace(configs.DebuggingLevel) || !Enum.TryParse(configs.DebuggingLevel, out LoggingLevel configLoggingLevel)
                ? fallbackLoggingLevel
                : configLoggingLevel;
        }
    }
}
