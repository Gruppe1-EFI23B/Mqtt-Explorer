using System;
using System.Windows;
using Microsoft.Data.Sqlite;
using Mqtt_Explorer.ViewModels;

namespace Mqtt_Explorer
{
    /// <summary>
    /// Hauptfenster der Anwendung
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            


            // Setze das DataContext auf das ViewModel
            this.DataContext = new MqttViewModel();

        }
    }
}
