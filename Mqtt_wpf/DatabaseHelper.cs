
using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace Mqtt_Explorer.Helpers
{
    public class DatabaseHelper
    {
        private readonly string dbPath;

        public DatabaseHelper()
        {
            dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datenbank.db");
        }

        public void EnsureDatabaseAndTable()
        {
            try
            {
                using var connection = new SqliteConnection($"Data Source={dbPath}");
                connection.Open();

                // Prüfen, ob die Tabelle existiert
                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = @"
                    SELECT name FROM sqlite_master 
                    WHERE type='table' AND name='Topics';
                ";

                var result = checkCmd.ExecuteScalar();

                if (result == null)
                {
                    Console.WriteLine("Tabelle 'Topics' nicht gefunden. Erstelle sie jetzt...");

                    var createCmd = connection.CreateCommand();
                    createCmd.CommandText = @"
                        CREATE TABLE Topics (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            topic TEXT NOT NULL
                        );
                    ";
                    createCmd.ExecuteNonQuery();

                    var insertCmd = connection.CreateCommand();
                    insertCmd.CommandText = @"
                        INSERT INTO Topics (topic) VALUES
                        ('test/topic'),
                        ('sensor/temperature'),
                        ('home/livingroom/light');
                    ";
                    insertCmd.ExecuteNonQuery();

                    Console.WriteLine("Tabelle 'Topics' wurde erstellt und befüllt.");
                }
                else
                {
                    Console.WriteLine("Tabelle 'Topics' existiert bereits.");
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler bei der Tabellenprüfung: {ex.Message}");
            }
        }

        public string GetDatabasePath() => dbPath;
    }
}
