using MoonrakerAPI.API;

namespace MoonrakerAPI.WebsocketNotifications;
    
public class KlippyShutdown : JsonRpc
{
    public const string MethodName = "notify_klippy_shutdown";
    
    public KlippyShutdown(JsonRpc rpc)
    {
        JsonRpcVersion = rpc.JsonRpcVersion;
        Method = rpc.Method;
        
        if(Method != MethodName)
            throw new ArgumentException($"Method must be \"{MethodName}\"");
    }
}