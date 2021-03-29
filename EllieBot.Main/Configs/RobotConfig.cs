using Newtonsoft.Json;
using System;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading.Tasks;
using static EllieBot.Constants;

namespace EllieBot.Configs {

    public class MqttConnectionDescription {
        public int Port { get; set; }
        public string Host { get; set; }
        public string TopicForCommands { get; set; }
        public string TopicForLogging { get; set; }
    }

    public class RobotConfig {
        public string DebuggingLevel { get; set; } = LoggingLevel.INFO.ToString();

        public MqttConnectionDescription MqttDefinitions = new MqttConnectionDescription {
            Port = Constants.Mqtt.PORT,
            Host = Constants.Mqtt.HOST,
            TopicForCommands = Constants.Mqtt.TOPIC_FOR_COMMANDS,
            TopicForLogging = Constants.Mqtt.TOPIC_FOR_LOGGING
        };

        public DriveTrainDescription DriveTrainDefinitions = new DriveTrainDescription {
            LeftMotorUniqueId = Constants.ComponentIds.LEFT_MOTOR,
            RightMotorUniqueId = Constants.ComponentIds.RIGHT_MOTOR
        };

        public HBridgeMotorDescription[] HBridgeMotorDefinitions = new HBridgeMotorDescription[] {
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

        public LedDescription[] LedDefinitions = new LedDescription[] {
            new LedDescription {
                UniqueId = Constants.ComponentIds.HEAD_LIGHTS,
                PinNumber = Constants.PinNums.HEAD_LIGHTS
            },
            new LedDescription {
                UniqueId = Constants.ComponentIds.BRAKE_LIGHTS,
                PinNumber = Constants.PinNums.BRAKE_LIGHTS
            },
            new LedDescription {
                UniqueId = Constants.ComponentIds.OTHER_LIGHTS,
                PinNumber = Constants.PinNums.OTHER_LIGHTS
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
