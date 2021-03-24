using EllieBot.Ambulator;
using EllieBot.Brain;
using EllieBot.Brain.Commands;
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
        private readonly IMotorsController motorController;
        private readonly RobotConfig configs;

        public Robot(ICommandProcessor cmdProcessor,
                     IMotorsController motorController,
                     RobotConfig configs) {
            this.commandProcessor = cmdProcessor;
            this.motorController = motorController;
            this.configs = configs;
        }

        public Task Initialize() {
            // Register All Desired Commands
            this.commandProcessor.RegisterCommand(new GoRawMotorControl(this.motorController));
            this.commandProcessor.RegisterCommand(new GoTankMotorControl(this.motorController));
            this.commandProcessor.RegisterCommand(new GoInterpretedMotorControl(this.motorController));

            this.comms = new Communications.NervousSystem();
            this.comms.ConnectAsync(this.configs.BackboneServer, this.configs.BackbonePort).Wait();

            return this.comms.SubscribeAsync(this.configs.TopicForCommands, this.OnDataReceived, this.OnConnection, this.OnDisconnection);
        }

        public Task PublishAsync(string message) {
            return this.comms.PublishAsync(this.configs.TopicForCommands, message);
        }

        private Task OnDisconnection(MqttClientDisconnectedEventArgs arg) {
            return Task.Run(() => {
                Console.WriteLine("Client Disconnected");
            });
        }

        private Task OnConnection(MqttClientConnectedEventArgs arg) {
            return Task.Run(() => {
                Console.WriteLine("Client Connected");
            });
        }

        private Task OnDataReceived(MqttApplicationMessageReceivedEventArgs arg) {
            return Task.Run(() => {
                string Payload = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                try {
                    RobotCommand cmd = JsonConvert.DeserializeObject<RobotCommand>(Payload);
                    this.commandProcessor.QueueExecute(cmd);
                } catch (Exception) {
                    Console.WriteLine($"Ignored: {Payload}");
                }
            });
        }
    }
}
