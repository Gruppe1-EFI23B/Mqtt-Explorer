﻿// MqttViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
}
