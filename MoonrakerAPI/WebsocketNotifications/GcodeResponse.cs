using System.Text.Json;
using System.Text.Json.Serialization;
using MoonrakerAPI.API;

namespace MoonrakerAPI.WebsocketNotifications;

public class GcodeResponse : JsonRpc
{
    public const string MethodName = "notify_gcode_response";

    public List<string> Params { get; set; }

    public GcodeResponse(JsonRpc rpc)
    {
        JsonRpcVersion = rpc.JsonRpcVersion;
        Method = rpc.Method;
        
        
        if(Method != MethodName)
            throw new ArgumentException($"Method must be \"{MethodName}\"");

        var paramsResult = JsonSerializer.Deserialize<List<string>>(rpc.RawParams.ToString());
        Params = paramsResult ?? throw new ArgumentException("Unable to parse GcodeResponseParams");
    }
}