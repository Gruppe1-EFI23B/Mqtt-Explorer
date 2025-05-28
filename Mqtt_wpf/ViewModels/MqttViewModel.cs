// MqttViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
    string topic;

    [ObservableProperty]
    string message;

    public MqttViewModel()
    {
        mqttModel = new();
        mqttModel.MessageReceived += (message) => ReceivedMessage += message + "\n";
        mqttModel.StatusUpdated += (status) => StatusMessage += status + "\n";


        var sql = @"CREATE TABLE Topics(
            id INTEGER PRIMARY KEY,
            Topic TEXT NOT NULL,
            )";

        var sql2 = @"Insert into Topics values (1,'Maschine01')";

        try
        {
            using var connection = new SqliteConnection(@"Data Source=H:\Desktop\DB\pub.db");
            connection.Open();

            using var command = new SqliteCommand(sql, connection);
            using var command2 = new SqliteCommand(sql2, connection);
            command.ExecuteNonQuery();
            command2.ExecuteNonQuery();

            Console.WriteLine("Table 'Topics' created successfully.");



        }
        catch (SqliteException ex)
        {
            Console.WriteLine(ex.Message);
        }
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
    public async Task TopicAsync()
    {
        await mqttModel.TopicAsync(Topic, Message);
    }
}
