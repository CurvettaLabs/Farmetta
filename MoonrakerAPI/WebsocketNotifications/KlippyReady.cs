using MoonrakerAPI.API;

namespace MoonrakerAPI.WebsocketNotifications;

public class KlippyReady : JsonRpc
{
    public const string MethodName = "notify_klippy_ready";
    
    public KlippyReady(JsonRpc rpc)
    {
        JsonRpcVersion = rpc.JsonRpcVersion;
        Method = rpc.Method;
        
        if(Method != MethodName)
            throw new ArgumentException($"Method must be \"{MethodName}\"");
    }
}