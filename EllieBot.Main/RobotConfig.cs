using Newtonsoft.Json;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading.Tasks;

namespace EllieBot {

    public class RobotConfig {
        public int BackbonePort { get; set; } = Constants.DEFAULT_COMMUNICATIONS_PORT;
        public string BackboneServer { get; set; } = Constants.DEFAULT_COMMUNICATIONS_ADDRESS;
        public int LeftMotorBackwardPin { get; internal set; } = Constants.DEFAULT_BACKWARD_PIN_LEFT;
        public int LeftMotorForwardPin { get; internal set; } = Constants.DEFAULT_FORWARD_PIN_LEFT;
        public int RightMotorBackwardPin { get; internal set; } = Constants.DEFAULT_BACKWARD_PIN_RIGHT;
        public int RightMotorForwardPin { get; internal set; } = Constants.DEFAULT_FORWARD_PIN_RIGHT;
        public int HeadlightsPin { get; internal set; } = Constants.DEFAULT_HEADLIGHTS_PIN;
        public string TopicForCommands { get; set; } = Constants.DEFAULT_TOPIC_FOR_COMMANDS;
        public string TopicForLogs { get; set; } = Constants.DEFAULT_TOPIC_FOR_LOGS;

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
