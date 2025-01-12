using Microsoft.AspNetCore.Components;
using MoonrakerAPI;

namespace Farmetta.Components.Pages;

public partial class MoonrakerAPITest : ComponentBase
{
    private string _status = "Disconnected";
    
    private string messages = string.Empty;

    private async Task Connect()
    {
        MoonrakerClient moonrakerClient = new(new Uri("ws://virtual-printer:7125/websocket"));
        moonrakerClient.MessageReceived += MessageReceived;
        
        await moonrakerClient.Connect();
        
        _status = "Connected";
    }

    private async void MessageReceived(object? sender, string message)
    {
        messages = $"{message}\n{messages}";
        
        await InvokeAsync(StateHasChanged);
    }
}