using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;

namespace EllieBot.Communications
{
    public class NervousSystem
    {
       
        public IManagedMqttClient Client { get; set; }

        public async Task ConnectAsync(string ipAddress, int port)
        {
            string clientId = Guid.NewGuid().ToString();

            MqttClientOptionsBuilder messageBuilder = new MqttClientOptionsBuilder()
              .WithClientId(clientId)
              .WithTcpServer(ipAddress, port)
              .WithCleanSession();

            IMqttClientOptions options = messageBuilder.Build();

            ManagedMqttClientOptions managedOptions = new ManagedMqttClientOptionsBuilder()
              .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
              .WithClientOptions(options)
              .Build();

            Client = new MqttFactory().CreateManagedMqttClient();

            await Client.StartAsync(managedOptions);
        }

        public Task PublishAsync(string topic, string payload, bool retainFlag = true, int qos = 1)
        {
            return Client.PublishAsync(new MqttApplicationMessageBuilder()
                                         .WithTopic(topic)
                                         .WithPayload(payload)
                                         .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
                                         .WithRetainFlag(retainFlag)
                                         .Build());
        }

        public Task SubscribeAsync(string topic,
                                   Func<MqttApplicationMessageReceivedEventArgs, Task> receiveHandler,
                                   Func<MQTTnet.Client.Connecting.MqttClientConnectedEventArgs, Task> connectedHandler,
                                   Func<MQTTnet.Client.Disconnecting.MqttClientDisconnectedEventArgs, Task> disconnectedHandler)
        {
            Client.UseApplicationMessageReceivedHandler(receiveHandler)
                .UseConnectedHandler(connectedHandler)
                .UseDisconnectedHandler(disconnectedHandler);
            return Client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
        }
    }
}
