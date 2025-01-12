using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace MoonrakerAPI.WebsocketNotifications;

public class WebsocketNotifications
{
    [JsonPropertyName("jsonrpc")]
    public string Jsonrpc { get; set; }
    
    [JsonPropertyName("method")]
    public string Method { get; set; }
}