using Microsoft.Extensions.Options;
using MQTTnet.Client;
using MQTTnet;
using System;
using System.Threading.Tasks;
using TMAMS_Data_Transmitter.Requests;

namespace TMAMS_Data_Transmitter.Services
{
    public class MqttClientService
    {
        private readonly IMqttClient _mqttClient;
        private readonly dynamic _mqttOptions;

        public MqttClientService(IOptions<MqttSettings> mqttSettings)
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
            var settings = mqttSettings.Value;
            _mqttOptions = new MqttClientOptionsBuilder()
                .WithClientId(settings.ClientId)
                .WithTcpServer(settings.Broker, settings.Port)
                .WithCleanSession(true)
                .Build();
        }

        public async Task ConnectAsync()
        {
            if (!_mqttClient.IsConnected)
            {
                await _mqttClient.ConnectAsync(_mqttOptions, System.Threading.CancellationToken.None);
            }
        }

        public async Task<bool> PublishAsync(string topic, string payload)
        {
            try
            {
                if (!_mqttClient.IsConnected)
                {
                    await ConnectAsync();
                }

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce)
                    .WithRetainFlag(true)
                    .Build();

                await _mqttClient.PublishAsync(message, System.Threading.CancellationToken.None);
                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return false;
            }
        }
    }
}
