using Newtonsoft.Json;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading.Tasks;

namespace EllieBot
{
    public class RobotConfig
    {
        public int BackbonePort { get; set; } = EllieBot.NervousSystem.Constants.DEFAULT_COMMUNICATIONS_PORT;
        public string BackboneServer { get; set; } = EllieBot.NervousSystem.Constants.DEFAULT_COMMUNICATIONS_ADDRESS;
        public int LeftMotorBackwardPin { get; internal set; } = EllieBot.Ambulator.Constants.DEFAULT_BACKWARD_PIN_RIGHT;
        public int LeftMotorForwardPin { get; internal set; } = EllieBot.Ambulator.Constants.DEFAULT_FORWARD_PIN_RIGHT;
        public int RightMotorBackwardPin { get; internal set; } = EllieBot.Ambulator.Constants.DEFAULT_BACKWARD_PIN_LEFT;
        public int RightMotorForwardPin { get; internal set; } = EllieBot.Ambulator.Constants.DEFAULT_FORWARD_PIN_LEFT;
        public string TopicForCommands { get; set; } = EllieBot.NervousSystem.Constants.DEFAULT_TOPIC_FOR_COMMANDS;

        internal static Task<RobotConfig> LoadFile(string fileName, IFileSystem fileSystem, System.Action<string> callback = null)
        {
            return Task.Run(() =>
            {
                string assyLoc = fileSystem.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string fullPath = fileSystem.Path.Combine(assyLoc, fileName);
                if (!fileSystem.File.Exists(fullPath))
                {
                    callback?.Invoke($"Saving new config file to {fullPath}");
                    RobotConfig config = GetDefaultRobotConfig();
                    config.SaveFile(fileName, fileSystem).Wait();
                    return config;
                }
                else
                {
                    callback?.Invoke($"Using existing config file in {fullPath}");
                    string json = fileSystem.File.ReadAllText(fileName);
                    return JsonConvert.DeserializeObject<RobotConfig>(json);
                }
            });
        }

        internal Task SaveFile(string fileName, IFileSystem fileSystem)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            return fileSystem.File.WriteAllTextAsync(fileName, json);
        }

        private static RobotConfig GetDefaultRobotConfig()
        {
            RobotConfig config = new RobotConfig();
            return config;
        }
    }
}