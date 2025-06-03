using System;
using System.Data;
using System.Data.Entity;
using System.Windows;
using Mqtt_Explorer.ViewModels;
using Microsoft.Data.Sqlite;
using Mqtt_Explorer.Helpers;

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

    }
}
