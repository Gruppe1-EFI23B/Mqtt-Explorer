using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
public class MessageRecord
{
    public int id { get; set; }
    public string topic { get; set; }
    public string message { get; set; }
    public DateTime timestamp { get; set; }
}


