using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Mqtt_wpf
{

    public class DatabaseInitializer
    {
        private readonly string dbPath;

        public DatabaseInitializer()
        {
            dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datenbank.db");
        }

        public void EnsureDatabaseCreated()
        {
            if (!File.Exists(dbPath))
            {
                try
                {
                    using var connection = new SqliteConnection($"Data Source={dbPath}");
                    connection.Open();

                    var createTableCmd = connection.CreateCommand();
                    createTableCmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Topics (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            topic TEXT NOT NULL
                        );
                    ";
                    createTableCmd.ExecuteNonQuery();

                    var insertCmd = connection.CreateCommand();
                    insertCmd.CommandText = @"
                        INSERT INTO Topics (topic) VALUES
                        ('test/topic'),
                        ('sensor/temperature'),
                        ('home/livingroom/light');
                    ";
                    insertCmd.ExecuteNonQuery();

                    connection.Close();
                    Console.WriteLine("Datenbank erfolgreich erstellt.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Erstellen der Datenbank: {ex.Message}");
                }
            }
        }

        public string GetDatabasePath() => dbPath;
    }

}
