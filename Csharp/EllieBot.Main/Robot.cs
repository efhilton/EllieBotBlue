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
        private readonly RobotConfig configs;

        public Robot(ICommandProcessor cmdProcessor, RobotConfig configs)
        {
            this.commandProcessor = cmdProcessor;
            this.configs = configs;
        }

        public Task Initialize()
        {
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
                Console.WriteLine($"ClientConnected = {arg.ClientWasConnected}");
                Console.WriteLine($"Reason = {arg.Reason}");
            });
        }

        private Task OnConnection(MqttClientConnectedEventArgs arg)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"AuthData = {arg.AuthenticateResult.AuthenticationData}");
                Console.WriteLine($"AuthMethod = {arg.AuthenticateResult.AuthenticationMethod}");
            });
        }

        private Task OnDataReceived(MqttApplicationMessageReceivedEventArgs arg)
        {
            return Task.Run(() =>
             {
                 string Payload = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                 Console.WriteLine($"{arg.ApplicationMessage.Topic}: {Payload}");

                 RobotCommand cmd = JsonConvert.DeserializeObject<RobotCommand>(Payload);
                 commandProcessor.QueueExecute(cmd);
             });
        }
    }
}