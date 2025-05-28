// MqttModel.cs
using MQTTnet;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

public class MqttModel
{
    private readonly HttpClient _httpClient = new();
    private readonly string _apiUrl = "http://localhost:8000/mqtt/topic"; // ggf. URL anpassen
    static int counter = 1;
    public IMqttClient client;

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

    public class MqttMessageCreate
    {
        public string topic { get; set; }
        public string payload { get; set; }
        public string timestamp { get; set; }
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

            // Passendes Objekt für FastAPI bauen
            var apiMessage = new MqttMessageCreate
            {
                topic = topic,
                payload = message,
                timestamp = DateTime.UtcNow.ToString("o")
            };

            var response = await _httpClient.PostAsJsonAsync(_apiUrl, apiMessage);
            counter++;
            StatusUpdated?.Invoke($"Message published to topic {topic}: {message}");
        }
    }
}