using System;
using System.Threading.Tasks;

namespace EllieBot.Logging {

    public class MqttLogger {

        public delegate Task Log(string message);

        public Constants.LoggingLevel MinLevel { get; set; } = Constants.LoggingLevel.FINE;
        public Log LogMessageFcn { get; set; } = (msg) => Task.Run(() => Console.Out.WriteLine(msg));

        public void Fine(string message) {
            if (this.MinLevel <= Constants.LoggingLevel.FINE) {
                this.LogMessageFcn($"Fine: {message}").Wait();
            }
        }

        public void Info(string message) {
            if (this.MinLevel <= Constants.LoggingLevel.INFO) {
                this.LogMessageFcn($"Info: {message}").Wait();
            }
        }

        public void Debug(string message) {
            if (this.MinLevel <= Constants.LoggingLevel.DEBUG) {
                this.LogMessageFcn($"Debug: {message}").Wait();
            }
        }

        public void Error(string message) {
            if (this.MinLevel <= Constants.LoggingLevel.ERROR) {
                this.LogMessageFcn($"Error: {message}").Wait();
            }
        }
    }
}
