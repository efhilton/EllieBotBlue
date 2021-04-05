namespace EllieBot.Configs {

    public class MqttConnectionDescription {
        public int Port { get; set; }
        public string Host { get; set; }
        public string TopicForCommands { get; set; }
        public string TopicForLogging { get; set; }
        public string TopicForSensorData { get; set; }
    }
}
