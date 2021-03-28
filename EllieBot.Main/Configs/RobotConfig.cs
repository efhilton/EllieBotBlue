using Newtonsoft.Json;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading.Tasks;

namespace EllieBot.Configs {

    public class MqttConnectionDescription {
        public int Port { get; set; }
        public string Host { get; set; }
        public string TopicForCommands { get; set; }
        public string TopicForLogging { get; set; }
    }

    public class RobotConfig {

        public MqttConnectionDescription MqttDefinitions = new MqttConnectionDescription {
            Port = Defaults.Mqtt.PORT,
            Host = Defaults.Mqtt.HOST,
            TopicForCommands = Defaults.Mqtt.TOPIC_FOR_COMMANDS,
            TopicForLogging = Defaults.Mqtt.TOPIC_FOR_LOGGING
        };

        public DriveTrainDescription DriveTrainDefinitions = new DriveTrainDescription {
            LeftMotorUniqueId = Defaults.ComponentIds.LEFT_MOTOR,
            RightMotorUniqueId = Defaults.ComponentIds.RIGHT_MOTOR
        };

        public HBridgeMotorDescription[] HBridgeMotorDefinitions = new HBridgeMotorDescription[] {
            new HBridgeMotorDescription {
                UniqueId = Defaults.ComponentIds.LEFT_MOTOR,
                ForwardPin = Defaults.PinNums.MOTOR_FORWARD_LEFT,
                BackwardPin = Defaults.PinNums.MOTOR_BACKWARD_LEFT
            },
            new HBridgeMotorDescription {
                UniqueId = Defaults.ComponentIds.RIGHT_MOTOR,
                ForwardPin = Defaults.PinNums.MOTOR_FORWARD_RIGHT,
                BackwardPin = Defaults.PinNums.MOTOR_BACKWARD_LEFT
            }
        };

        public LedDescription[] LedDefinitions = new LedDescription[] {
            new LedDescription {
                UniqueId = Defaults.ComponentIds.HEAD_LIGHTS,
                PinNumber = Defaults.PinNums.HEAD_LIGHTS
            },
            new LedDescription {
                UniqueId = Defaults.ComponentIds.BRAKE_LIGHTS,
                PinNumber = Defaults.PinNums.BRAKE_LIGHTS
            },
            new LedDescription {
                UniqueId = Defaults.ComponentIds.OTHER_LIGHTS,
                PinNumber = Defaults.PinNums.OTHER_LIGHTS
            }
        };

        internal static Task<RobotConfig> LoadFile(string fileName, IFileSystem fileSystem, System.Action<string> callback = null) {
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
