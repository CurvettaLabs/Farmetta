using KlipperInstaller;
using Microsoft.AspNetCore.Components;
using MoonrakerAPI;

namespace Farmetta.Components.Widgets;

public partial class MoonrakerClientsList : ComponentBase, IDisposable
{
    [Inject]
    private MoonrakerInstanceManager _moonrakerInstanceManager { get; set; } = null!;

    [Inject]
    private KlipperInstanceFactory KlipperInstanceFactory { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        _moonrakerInstanceManager.OnMoonrakerClientCreated += OnMoonrakerClientCreated;
        _moonrakerInstanceManager.OnMoonrakerClientConnected += OnMoonrakerClientConnected;
        _moonrakerInstanceManager.OnMoonrakerClientDisconnected += OnMoonrakerClientDisconnected;
        _moonrakerInstanceManager.OnMoonrakerClientRemoved += OnMoonrakerClientRemoved;
    }

    private void OnMoonrakerClientRemoved(string clientname, MoonrakerClient moonrakerclient)
    {
        InvokeAsync(StateHasChanged);
    }

    private void OnMoonrakerClientCreated(string clientname, MoonrakerClient moonrakerclient)
    {
        InvokeAsync(StateHasChanged);
    }


    private void OnMoonrakerClientConnected(string clientname, MoonrakerClient moonrakerclient)
    {
        InvokeAsync(StateHasChanged);
    }
    
    private void OnMoonrakerClientDisconnected(string clientname, MoonrakerClient moonrakerclient)
    {
        InvokeAsync(StateHasChanged);
    }

    private async Task RemoveMoonrakerClient(string clientname)
    {
        await _moonrakerInstanceManager.RemoveClient(clientname);
    }

    private Task TestKlipperInit()
    {
        return KlipperInstanceFactory.UpdateDeps();
    }

    public void Dispose()
    {
        _moonrakerInstanceManager.OnMoonrakerClientCreated -= OnMoonrakerClientCreated;
        _moonrakerInstanceManager.OnMoonrakerClientConnected -= OnMoonrakerClientConnected;
        _moonrakerInstanceManager.OnMoonrakerClientDisconnected -= OnMoonrakerClientDisconnected;
        _moonrakerInstanceManager.OnMoonrakerClientRemoved -= OnMoonrakerClientRemoved;
    }
}