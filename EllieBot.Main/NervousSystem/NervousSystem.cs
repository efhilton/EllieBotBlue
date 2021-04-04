using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Threading.Tasks;

namespace EllieBot.Communications {

    public class NervousSystem {
        public IManagedMqttClient Client { get; set; }

        public Task ConnectAsync(string ipAddress, int port) {
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

            this.Client = new MqttFactory().CreateManagedMqttClient();

            return this.Client.StartAsync(managedOptions);
        }

        public Task PublishAsync(string topic, string payload, bool retainFlag = false, int qos = 0) {
            return this.Client.PublishAsync(new MqttApplicationMessageBuilder()
                                         .WithTopic(topic)
                                         .WithPayload(payload)
                                         .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
                                         .WithRetainFlag(retainFlag)
                                         .Build());
        }

        public Task SubscribeAsync(string topic,
                                   Func<MqttApplicationMessageReceivedEventArgs, Task> onMessageHandler,
                                   Func<MQTTnet.Client.Connecting.MqttClientConnectedEventArgs, Task> onConnectedHandler,
                                   Func<MQTTnet.Client.Disconnecting.MqttClientDisconnectedEventArgs, Task> onDisconnectedHandler) {
            this.Client.UseApplicationMessageReceivedHandler(onMessageHandler)
                .UseConnectedHandler(onConnectedHandler)
                .UseDisconnectedHandler(onDisconnectedHandler);
            return this.Client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
        }
    }
}
