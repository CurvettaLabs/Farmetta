using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using MoonrakerAPI.API;
using MoonrakerAPI.WebsocketNotifications;

namespace MoonrakerAPI;

public partial class MoonrakerClient
{
    public event EventHandler<ProcessStatisticUpdate> OnProcessStatisticUpdate;

    private bool TryReceiveWebsocketNotification(JsonRpc jsonRpc)
    {
        var method = jsonRpc.Method;

        switch (method)
        {
            case ProcessStatisticUpdate.MethodName:
                ProcessStatisticUpdate update = new(jsonRpc);
                OnProcessStatisticUpdate?.Invoke(this, update);
                break;
            default:
                return false;
        }
        return true;
    }
}