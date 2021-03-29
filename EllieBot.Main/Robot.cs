using EllieBot.Brain;
using EllieBot.Configs;
using EllieBot.Logging;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EllieBot {

    public class Robot {
        private Communications.NervousSystem comms;
        private readonly ICommandProcessor commandProcessor;
        private readonly RobotConfig configs;
        private readonly MqttLogger logger;

        public static Robot Instance { get; private set; }

        private Robot(ICommandProcessor cmdProcessor, RobotConfig configs, MqttLogger logger) {
            this.commandProcessor = cmdProcessor;
            this.configs = configs;
            this.logger = logger;
        }

        public Task Initialize() {
            this.comms = new Communications.NervousSystem();
            this.comms.ConnectAsync(this.configs.MqttDefinitions.Host, this.configs.MqttDefinitions.Port).Wait();

            return this.comms.SubscribeAsync(this.configs.MqttDefinitions.TopicForCommands, this.OnCommandReceived, this.OnCommandConnection, this.OnCommandDisconnection);
        }

        internal static Robot CreateInstance(ICommandProcessor proc, RobotConfig configs, MqttLogger logger) {
            if (Instance != null) {
                return Instance;
            }
            Instance = new Robot(proc, configs, logger);
            return Instance;
        }

        public Task PublishCommandAsync(string message) => this.comms.PublishAsync(this.configs.MqttDefinitions.TopicForCommands, message);

        public Task PublishLogAsync(string message) => this.comms.PublishAsync(this.configs.MqttDefinitions.TopicForLogging, message);

        private Task OnCommandDisconnection(MqttClientDisconnectedEventArgs arg) => Task.Run(() => this.logger.Info("Client Disconnected"));

        private Task OnCommandConnection(MqttClientConnectedEventArgs arg) => Task.Run(() => this.logger.Info("Client Connected"));

        private Task OnCommandReceived(MqttApplicationMessageReceivedEventArgs arg) {
            return Task.Run(() => {
                string Payload = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                try {
                    CommandPacket cmd = JsonConvert.DeserializeObject<CommandPacket>(Payload);
                    this.commandProcessor.QueueExecute(cmd);
                } catch (Exception) {
                    this.logger.Debug($"Ignored: {Payload}");
                }
            });
        }
    }
}
