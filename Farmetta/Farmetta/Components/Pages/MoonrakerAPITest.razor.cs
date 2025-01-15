using Microsoft.AspNetCore.Components;
using MoonrakerAPI;
using MoonrakerAPI.WebsocketNotifications;

namespace Farmetta.Components.Pages;

public partial class MoonrakerAPITest : ComponentBase
{
    private string _status = "Disconnected";
    private string moonrakerIp = string.Empty;
    private string moonrakerPort = string.Empty;
    private string moonrakerInstanceName = string.Empty;

    private ProcessStatisticUpdate? _processStatistics;
    
    private MoonrakerClient? _moonrakerClient;
    
    [Inject]
    private MoonrakerInstanceManager MoonrakerInstanceManager { get; set; } = null!;

    private async Task AddMoonrakerInstance()
    {
        var uri = new Uri($"ws://{moonrakerIp}:{moonrakerPort}/websocket");
        _moonrakerClient = await MoonrakerInstanceManager.CreateMoonrakerClient(moonrakerInstanceName, uri);
    }
    

    private async Task Connect()
    {
        MoonrakerClient moonrakerClient = new(new Uri("ws://virtual-printer:7125/websocket"));
        moonrakerClient.OnProcessStatisticUpdate += ProcessStatusUpdateReceived;
        
        await moonrakerClient.Connect();
        
        _status = "Connected";
    }

    private void ProcessStatusUpdateReceived(object? sender, ProcessStatisticUpdate processStatisticUpdate)
    {
        _processStatistics = processStatisticUpdate;
        InvokeAsync(StateHasChanged);
    }
    
    
}