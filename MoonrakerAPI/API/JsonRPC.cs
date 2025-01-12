using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MoonrakerAPI.API;

public class JsonRpc
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpcVersion { get; set; }
    
    [JsonPropertyName("method")]
    public string Method { get; set; }
    
    [JsonPropertyName("params")]
    public JsonElement RawParams { get; set; }
}