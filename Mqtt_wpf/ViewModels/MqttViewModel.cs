// MqttViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;
using Mqtt_Explorer;
using Mqtt_wpf;

namespace Mqtt_Explorer.ViewModels;
public partial class MqttViewModel : ObservableObject
{
    private MqttModel mqttModel;

    [ObservableProperty]
    string brokerAddress;

    [ObservableProperty]
    string receivedMessage;

    [ObservableProperty]
    string statusMessage;

    [ObservableProperty]
    ObservableCollection<string> topics = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PublishEnabled))]
    string topic;

    [NotifyPropertyChangedFor(nameof(PublishEnabled))]
    [ObservableProperty]
    string message;



    public bool PublishEnabled => mqttModel.client.IsConnected && !string.IsNullOrWhiteSpace(Topic) && !string.IsNullOrWhiteSpace(Message);

    public MqttViewModel()
    {

        var dbInit = new DatabaseInitializer();
        dbInit.EnsureDatabaseCreated();

        mqttModel = new();
        mqttModel.MessageReceived += (message) => ReceivedMessage += message + "\n";
        mqttModel.StatusUpdated += StatusUpdated;
    }

    public void LoadTopicsFromDatabase()
    {
        try
        {
            // Dynamischer Pfad zur Datenbank relativ zum Programmverzeichnis
            var dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datenbank.db");
            var connectionString = $"Data Source={dbPath}";
            StatusMessage += $"Verwendeter Pfad: {dbPath}\n";

            var sql = @"SELECT topic FROM Topics";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();

            Topics.Clear();
            while (reader.Read())
            {
                var topic = reader.GetString(0);
                Topics.Add(topic);
            }

            if (Topics.Count > 0)
            {
                Topic = Topics[0]; // optional: erstes Topic vorauswählen
            }
        }
        catch (SqliteException ex)
        {
            StatusMessage += $"Fehler beim Laden der Topics: {ex.Message}\n";
        }
    }

    private void StatusUpdated(string status)
    {
        StatusMessage += status + "\n";
        OnPropertyChanged(nameof(PublishEnabled));
    }

    [RelayCommand]
    public async Task ConnectAsync()
    {
        await mqttModel.ConnectAsync(BrokerAddress, Guid.NewGuid().ToString());
    }

    [RelayCommand]
    public async Task DisconnectAsync()
    {
        await mqttModel.DisconnectAsync();
    }

    [RelayCommand]
    public async Task SubscribeAsync()
    {
        await mqttModel.SubscribeAsync(Topic);
    }

    [RelayCommand]
    public async Task PublishAsync()
    {
        await mqttModel.PublishAsync(Topic, Message);
    }

    [RelayCommand]
    public async Task UnsubscribeAsync()
    {
        await mqttModel.UnsubscribeAsync(Topic);
    }
}
