using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;

namespace EllieBot.NervousSystem
{
    public class CommunicationBroker
    {
        public static string COMMAND_TOPIC = "elliebot/commands";

        public IManagedMqttClient Client { get; set; }

        public async Task ConnectAsync()
        {
            string clientId = Guid.NewGuid().ToString();
            string mqttURI = "192.168.1.135";
            //string mqttUser = "elliebot";
            //string mqttPassword = "rocks3000";
            int mqttPort = 1883;
            bool mqttSecure = false;


            MqttClientOptionsBuilder messageBuilder = new MqttClientOptionsBuilder()
              .WithClientId(clientId)
              // .WithCredentials(mqttUser, mqttPassword)
              .WithTcpServer(mqttURI, mqttPort)
              .WithCleanSession();

            IMqttClientOptions options = mqttSecure
              ? messageBuilder                .WithTls()                .Build()
              : messageBuilder                .Build();

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

    }



}
