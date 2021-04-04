using Newtonsoft.Json;
using System;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading.Tasks;
using static EllieBot.Constants;

namespace EllieBot.Configs {

    public class RobotConfig {
        public string DebuggingLevel { get; set; } = LoggingLevel.INFO.ToString();

        public MqttConnectionDescription MqttDefinitions { get; set; } = new MqttConnectionDescription {
            Port = Constants.Mqtt.PORT,
            Host = Constants.Mqtt.HOST,
            TopicForCommands = Constants.Mqtt.TOPIC_FOR_COMMANDS,
            TopicForLogging = Constants.Mqtt.TOPIC_FOR_LOGGING
        };

        public DriveTrainDescription DriveTrainDefinitions { get; set; } = new DriveTrainDescription {
            LeftMotorUniqueId = Constants.ComponentIds.LEFT_MOTOR,
            RightMotorUniqueId = Constants.ComponentIds.RIGHT_MOTOR
        };

        public HBridgeMotorDescription[] HBridgeMotorDefinitions { get; set; } = new HBridgeMotorDescription[] {
            new HBridgeMotorDescription {
                UniqueId = Constants.ComponentIds.LEFT_MOTOR,
                ForwardPin = Constants.PinNums.MOTOR_FORWARD_LEFT,
                BackwardPin = Constants.PinNums.MOTOR_BACKWARD_LEFT
            },
            new HBridgeMotorDescription {
                UniqueId = Constants.ComponentIds.RIGHT_MOTOR,
                ForwardPin = Constants.PinNums.MOTOR_FORWARD_RIGHT,
                BackwardPin = Constants.PinNums.MOTOR_BACKWARD_LEFT
            }
        };

        public LedDescription[] LedDefinitions { get; set; } = new LedDescription[] {
            new LedDescription {
                UniqueId = Constants.ComponentIds.YELLOW_LIGHT,
                PinNumber = Constants.PinNums.YELLOW_LIGHT
            },
        };

        public Hcsr04sDescription[] Hcsr04sDescriptions { get; set; } = new Hcsr04sDescription[] {
            new Hcsr04sDescription {
                UniqueId = Constants.ComponentIds.ULTRASONIC_HCSR04,
                TriggerPin = Constants.PinNums.ULTRASONIC_HCSR04_TRIGGER,
                EchoPin = Constants.PinNums.ULTRASONIC_HCSR04_ECHO
            }
        };

        internal static Task<RobotConfig> LoadFile(string fileName, IFileSystem fileSystem, Action<string> callback = null) {
            return Task.Run(() => {
                string assyLoc = fileSystem.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string fullPath = fileSystem.Path.Combine(assyLoc, fileName);
                if (!fileSystem.File.Exists(fullPath)) {
                    callback?.Invoke($"Saving new config file to {fullPath}");
                    RobotConfig config = GetDefaultRobotConfig();
                    config.SaveFile(fullPath, fileSystem).Wait();
                    return config;
                } else {
                    callback?.Invoke($"Using existing config file in {fullPath}");
                    string json = fileSystem.File.ReadAllText(fullPath);
                    return JsonConvert.DeserializeObject<RobotConfig>(json);
                }
            });
        }

        internal Task SaveFile(string fileName, IFileSystem fileSystem) {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            return fileSystem.File.WriteAllTextAsync(fileName, json);
        }

        private static RobotConfig GetDefaultRobotConfig() {
            RobotConfig config = new RobotConfig();
            return config;
        }
    }
}
