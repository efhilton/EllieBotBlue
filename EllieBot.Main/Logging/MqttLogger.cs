using System;
using System.Threading.Tasks;

namespace EllieBot.Logging {

    public class MqttLogger : ILogger {

        public delegate Task Log(Constants.LoggingLevel level, string message);

        public Constants.LoggingLevel MinLevel { get; set; } = Constants.LoggingLevel.FINE;
        public Log LogMessageFcn { get; set; } = (level, msg) => Task.Run(() => Console.Out.WriteLine($"({level}) {msg}"));

        public void Finest(string message) {
            if (this.MinLevel <= Constants.LoggingLevel.FINEST) {
                this.LogMessageFcn(Constants.LoggingLevel.FINEST, message).Wait();
            }
        }

        public void Fine(string message) {
            if (this.MinLevel <= Constants.LoggingLevel.FINE) {
                this.LogMessageFcn(Constants.LoggingLevel.FINE, message).Wait();
            }
        }

        public void Debug(string message) {
            if (this.MinLevel <= Constants.LoggingLevel.DEBUG) {
                this.LogMessageFcn(Constants.LoggingLevel.DEBUG, message).Wait();
            }
        }

        public void Info(string message) {
            if (this.MinLevel <= Constants.LoggingLevel.INFO) {
                this.LogMessageFcn(Constants.LoggingLevel.INFO, message).Wait();
            }
        }

        public void Warn(string message) {
            if (this.MinLevel <= Constants.LoggingLevel.WARN) {
                this.LogMessageFcn(Constants.LoggingLevel.WARN, message).Wait();
            }
        }

        public void Error(string message) {
            if (this.MinLevel <= Constants.LoggingLevel.ERROR) {
                this.LogMessageFcn(Constants.LoggingLevel.ERROR, message).Wait();
            }
        }
    }
}
