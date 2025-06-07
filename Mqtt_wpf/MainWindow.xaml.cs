using Microsoft.Data.Sqlite;
using Mqtt_Explorer.Helpers;
using Mqtt_Explorer.ViewModels;
using MQTTnet;
using System;
using System.Data;
using System.Data.Entity;
using System.Windows;

namespace Mqtt_Explorer
{
    /// <summary>
    /// Hauptfenster der Anwendung
    /// </summary>
    public partial class MainWindow : Window
    {
        private MqttViewModel Vm => (MqttViewModel)this.DataContext;
        private bool isSubscribed = false;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MqttViewModel();
            var dbHelper = new DatabaseHelper();
            dbHelper.EnsureDatabaseAndTable();
            Vm.LoadTopicsFromDatabase(); // <-- Topics laden
            
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            
            var mqttModel = Vm.GetType().GetField("mqttModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(Vm) as MqttModel;
            var clientId = mqttModel.ClientId;

            if (mqttModel != null)
            {
                if (mqttModel.client != null && !mqttModel.client.IsConnected)
                {
                    await mqttModel.ConnectAsync(Vm.BrokerAddress, clientId);
                    var connected = mqttModel.client.IsConnected;
                    if (connected)
                    {
                        btn_Connect.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightSalmon);
                        btn_Connect.Content = "Disconnect";
                        btn_Subscribe.IsEnabled = true;                                 
                    }
                    else
                    {
                       
                    }             
                }
                else
                {
                    btn_Connect.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGreen);
                    btn_Connect.Content = "Connect";
                    btn_Subscribe.IsEnabled = false;
                    await mqttModel.DisconnectAsync();
                }
            }
        }

        private async void SubscribeClick(object sender, RoutedEventArgs e)
        {
            var mqttModel = Vm.GetType().GetField("mqttModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(Vm) as MqttModel;
            if (isSubscribed)
            {
                await mqttModel.UnsubscribeAsync(cbTopics.SelectedItem as string);
                btn_Subscribe.Content = "Subscribe";
                btn_Subscribe.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGreen);
                isSubscribed = false;
                btn_Publish.IsEnabled = true;
            }
            else
            {
               await mqttModel.SubscribeAsync(cbTopics.SelectedItem as string);
                btn_Subscribe.Content = "Unsubscribe";
                btn_Subscribe.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightSalmon);
                isSubscribed = true;
                btn_Publish.IsEnabled = false;
            }
        }
    }
}
