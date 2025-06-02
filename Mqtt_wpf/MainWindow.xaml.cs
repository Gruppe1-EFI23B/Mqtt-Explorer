using System;
using System.Data;
using System.Data.Entity;
using System.Windows;
using Datenbank;
using Mqtt_Explorer.ViewModels;
using Microsoft.Data.Sqlite;

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
            try
            {
                var sql = @"SELECT * FROM Topics";
                using var connection = new SqliteConnection("Data Source=C:/Users/loris/source/repos/Mqtt-Explorer/Datenbank.db");
                connection.Open();
                using var command = new SqliteCommand(sql, connection);

                using var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var topic = reader.GetString(1);
                        Console.WriteLine($"{id}\t{topic}");
                        cbTopics.Items.Add(topic);
                    }
                }
                else
                {
                    Console.WriteLine("No authors found.");
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }
                
        }
  
        // Zugriff auf das ViewModel über das DataContext
        private MqttViewModel Vm => (MqttViewModel)this.DataContext;
    }
}
