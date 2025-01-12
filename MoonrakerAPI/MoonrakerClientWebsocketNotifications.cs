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
    public event EventHandler<GcodeResponse> OnGcodeResponse;
    public event EventHandler<KlippyDisconnectedNotification> OnKlippyDisconnected;
    public event EventHandler<KlippyReady> OnKlippyReady;
    public event EventHandler<KlippyShutdown> OnKlippyShutdown;

    /**
     * Attempts to read the jsonRpc as a websocket notification.
     * <returns>True if able to read as websocket notification, False if otherwise.</returns>
     */
    private bool TryReceiveWebsocketNotification(JsonRpc jsonRpc)
    {
        var method = jsonRpc.Method;

        switch (method)
        {
            case GcodeResponse.MethodName:
                GcodeResponse gcodeResponse = new(jsonRpc);
                OnGcodeResponse?.Invoke(this, gcodeResponse);
                break;
            case KlippyDisconnectedNotification.MethodName:
                KlippyDisconnectedNotification klippyDisconnected = new(jsonRpc);
                OnKlippyDisconnected?.Invoke(this, klippyDisconnected);
                break;
            case KlippyReady.MethodName:
                KlippyReady klippyReady = new(jsonRpc);
                OnKlippyReady?.Invoke(this, klippyReady);
                break;
            case KlippyShutdown.MethodName:
                KlippyShutdown klippyShutdown = new(jsonRpc);
                OnKlippyShutdown?.Invoke(this, klippyShutdown);
                break;
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