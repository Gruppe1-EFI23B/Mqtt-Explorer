// MqttModel.cs
using MQTTnet;
using System;
using System.Text;
using System.Threading.Tasks;

public class MqttModel
{
    private IMqttClient client;

    public event Action<string> MessageReceived;
    public event Action<string> StatusUpdated;

    public MqttModel()
    {
        client = new MqttClientFactory().CreateMqttClient();
        client.ApplicationMessageReceivedAsync += e =>
        {
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            MessageReceived?.Invoke(message);
            return Task.CompletedTask;
        };
    }

    public async Task ConnectAsync(string brokerAddress, string clientId)
    {
        try
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(brokerAddress)
                .WithClientId(clientId)
                .Build();

            await client.ConnectAsync(options);
            StatusUpdated?.Invoke($"Connected to broker {brokerAddress}");
        }
        catch (Exception ex)
        {
            StatusUpdated?.Invoke($"Error: {ex.Message}");
        }
    }

    public async Task DisconnectAsync()
    {
        if (client.IsConnected)
        {
            await client.DisconnectAsync();
            StatusUpdated?.Invoke("Disconnected");
        }
    }

    public async Task SubscribeAsync(string topic)
    {
        if (client.IsConnected)
        {
            await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
            StatusUpdated?.Invoke($"Subscribed to topic: {topic}");
        }
    }

    public async Task PublishAsync(string topic, string message)
    {
        if (client.IsConnected)
        {
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(message)
                .Build();
            await client.PublishAsync(mqttMessage);
            StatusUpdated?.Invoke($"Message published to topic {topic}: {message}");
        }
    }

    public async Task TopicAsync(string topic, string message)
    {


    }
}
