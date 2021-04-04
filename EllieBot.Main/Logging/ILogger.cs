namespace EllieBot.Logging {

    public interface ILogger {
        MqttLogger.Log LogMessageFcn { get; set; }
        Constants.LoggingLevel MinLevel { get; set; }

        void Debug(string message);

        void Error(string message);

        void Fine(string message);

        void Finest(string message);

        void Info(string message);

        void Warn(string message);
    }
}
