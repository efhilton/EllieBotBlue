using EllieBot.Configs.Descriptions;
using Newtonsoft.Json;
using System;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading.Tasks;
using static EllieBot.Constants;

namespace EllieBot.Configs {

    public class RobotConfig {
        public string DebuggingLevel { get; set; } = LoggingLevel.INFO.ToString();

        public MqttConnectionDescription MqttConnectionDescription { get; set; } = new MqttConnectionDescription {
            Port = Mqtt.PORT,
            Host = Mqtt.HOST,
            TopicForCommands = Mqtt.TOPIC_FOR_COMMANDS,
            TopicForLogging = Mqtt.TOPIC_FOR_LOGGING,
            TopicForSensorData = Mqtt.TOPIC_FOR_SENSORS
        };

        public DriveTrainDescription DriveTrainDescription { get; set; } = new DriveTrainDescription {
            LeftMotorUniqueId = ComponentIds.LEFT_MOTOR,
            RightMotorUniqueId = ComponentIds.RIGHT_MOTOR
        };

        public Actuators Actuators { get; set; } = new Actuators();
        public Sensors Sensors { get; set; } = new Sensors();

        internal static Task<RobotConfig> LoadFile(string fileName, IFileSystem fileSystem, Action<string> callback = null) {
            return Task.Run(() => {
                string assyLoc = fileSystem.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string fullPath = fileSystem.Path.Combine(assyLoc, fileName);
                if (!fileSystem.File.Exists(fullPath)) {
                    callback?.Invoke($"Saving new config file to {fullPath}");
                    RobotConfig config = new RobotConfig();
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
    }
}
