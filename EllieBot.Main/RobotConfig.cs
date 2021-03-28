using Newtonsoft.Json;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading.Tasks;

namespace EllieBot {

    public class HBridgeMotorDescription {
        public string UniqueId { get; set; }
        public int ForwardPin { get; set; }
        public int BackwardPin { get; set; }
    }

    public class LedDescription {
        public string UniqueId { get; set; }
        public int PinNumber { get; set; }
    }

    public class RobotConfig {
        public int MqttPort { get; set; } = Defaults.Mqtt.PORT;
        public string MqttServer { get; set; } = Defaults.Mqtt.HOST;
        public string MqttTopicForCommands { get; set; } = Defaults.Mqtt.TOPIC_FOR_COMMANDS;
        public string MqttTopicForLogs { get; set; } = Defaults.Mqtt.TOPIC_FOR_LOGS;
        public string LeftMotorUniqueId { get; set; } = Defaults.ComponentIds.LEFT_MOTOR;
        public string RightMotorUniqueId { get; set; } = Defaults.ComponentIds.RIGHT_MOTOR;

        public HBridgeMotorDescription[] HBridgeMotorDescriptions = new HBridgeMotorDescription[] {
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

        public LedDescription[] LedDescriptions = new LedDescription[] {
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
