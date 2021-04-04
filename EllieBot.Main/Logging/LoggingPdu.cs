namespace EllieBot.Logging {

    public class LoggingPdu {
        public Constants.LoggingLevel Level { get; set; } = Constants.LoggingLevel.INFO;
        public string LevelStr => this.Level.ToString();
        public string Message { get; set; } = string.Empty;
    }
}
