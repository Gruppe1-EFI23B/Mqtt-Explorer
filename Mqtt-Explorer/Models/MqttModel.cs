using MQTTnet;
using MQTTnet.Client;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

public class MqttModel
{
    private IMqttClient client;
    private string dbPath = "messages.db";

    public event Action<string> MessageReceived;
    public event Action<string> StatusUpdated;

    public MqttModel()
    {
        client = new MqttFactory().CreateMqttClient();

        InitDatabase();

        client.ApplicationMessageReceivedAsync += e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            MessageReceived?.Invoke(message);
            SaveMessage(topic, message);
            return Task.CompletedTask;
        };
    }

    private void InitDatabase()
    {
        if (!File.Exists(dbPath))
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
                CREATE TABLE IF NOT EXISTS Messages (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Topic TEXT NOT NULL,
                    Message TEXT NOT NULL,
                    Timestamp TEXT NOT NULL
                );
            ";
            command.ExecuteNonQuery();
        }
    }

    private void SaveMessage(string topic, string message)
    {
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
            INSERT INTO Messages (Topic, Message, Timestamp)
            VALUES ($topic, $message, $timestamp);
        ";

        command.Parameters.AddWithValue("$topic", topic);
        command.Parameters.AddWithValue("$message", message);
        command.Parameters.AddWithValue("$timestamp", DateTime.UtcNow.ToString("o"));

        command.ExecuteNonQuery();
    }

    public async Task ConnectAsync(string brokerAddress, string clientId)
    {
        try
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId("clientId")
                .WithTcpServer("broker.hivemq.com")
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
        try
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                StatusUpdated?.Invoke("Fehler: Topic darf nicht leer sein.");
                return;
            }

            if (!client.IsConnected)
            {
                StatusUpdated?.Invoke("Fehler: Client ist nicht verbunden.");
                return;
            }

            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(message ?? string.Empty) // falls message null ist
                .Build();

            await client.PublishAsync(mqttMessage);

            StatusUpdated?.Invoke($"✅ Nachricht veröffentlicht auf Topic \"{topic}\": {message}");
        }
        catch (Exception ex)
        {
            StatusUpdated?.Invoke($"❌ Fehler beim Veröffentlichen: {ex.Message}");
            Console.WriteLine($"[PublishAsync] Exception: {ex}");
        }
    }
