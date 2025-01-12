using MoonrakerAPI.API;

namespace MoonrakerAPI.WebsocketNotifications;

public class KlippyDisconnectedNotification : JsonRpc
{
    public const string MethodName = "notify_klippy_disconnected";
    
    public KlippyDisconnectedNotification(JsonRpc rpc)
    {
        JsonRpcVersion = rpc.JsonRpcVersion;
        Method = rpc.Method;
        
        if(Method != MethodName)
            throw new ArgumentException("Method must be \"notify_proc_stat_update\"");
    }
}