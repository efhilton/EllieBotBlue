using EllieBot.Ambulator;
using EllieBot.Brain;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EllieBot
{
    public class Robot
    {
        private Communications.NervousSystem comms;
        private readonly ICommandProcessor commandProcessor;
        private readonly IMotorsController motorController;
        private readonly RobotConfig configs;

        public Robot(ICommandProcessor cmdProcessor,
                     IMotorsController motorController,
                     RobotConfig configs)
        {
            this.commandProcessor = cmdProcessor;
            this.motorController = motorController;
            this.configs = configs;
        }

        public Task Initialize()
        {
            this.commandProcessor.RegisterCommand("go", this.motorController);

            comms = new Communications.NervousSystem();
            comms.ConnectAsync(configs.BackboneServer, configs.BackbonePort).Wait();

            return comms.SubscribeAsync(configs.TopicForCommands, OnDataReceived, OnConnection, OnDisconnection);
        }

        public Task PublishAsync(string message)
        {
            return comms.PublishAsync(configs.TopicForCommands, message);
        }

        private Task OnDisconnection(MqttClientDisconnectedEventArgs arg)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Client Disconnected");
            });
        }

        private Task OnConnection(MqttClientConnectedEventArgs arg)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Client Connected");
            });
        }

        private Task OnDataReceived(MqttApplicationMessageReceivedEventArgs arg)
        {
            return Task.Run(() =>
             {
                string Payload = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                try { 
                    RobotCommand cmd = JsonConvert.DeserializeObject<RobotCommand>(Payload);
                    commandProcessor.QueueExecute(cmd);
                } 
                catch (Exception)
                {
                    Console.WriteLine($"Ignored: {Payload}");
                }
             });
        }
    }
}