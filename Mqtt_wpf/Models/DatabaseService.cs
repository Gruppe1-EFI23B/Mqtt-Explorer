using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

// Modell mit exakt passenden Feldnamen zu FastAPI
public class MqttMessageCreate
{
    [JsonPropertyName("topic")]
    public string topic { get; set; }
    [JsonPropertyName("payload")]
    public string payload { get; set; }
    [JsonPropertyName("timestamp")]
    public string timestamp { get; set; }
}

public class ApiService
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string _baseUrl = "http://localhost:8000";

    public async Task<bool> PublishMessageAsync(string topic, string payload)
    {
        var message = new MqttMessageCreate
        {
            topic = topic,
            payload = payload,
            timestamp = DateTime.UtcNow.ToString("o")
        };

        // Debug: Zeige das JSON, das gesendet wird
        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(message));

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/mqtt/topic", message);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("✅ Nachricht erfolgreich gepostet.");
                return true;
            }
            else
            {
                Console.WriteLine($"❌ Fehler beim Posten: {response.StatusCode}");
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Antwort: {content}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❗ Ausnahme: {ex.Message}");
            return false;
        }
    }
}
