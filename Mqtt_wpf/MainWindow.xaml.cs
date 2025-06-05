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
                    btn_Connect.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                    btn_Connect.Content = "Disconnect";
                    await mqttModel.ConnectAsync(Vm.BrokerAddress, clientId);
                }
                else
                {
                    btn_Connect.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                    btn_Connect.Content = "Connect";
                    await mqttModel.DisconnectAsync();
                }
            }
        }
    }
}
