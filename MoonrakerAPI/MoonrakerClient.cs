using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using MoonrakerAPI.API;

namespace MoonrakerAPI;

public partial class MoonrakerClient : IDisposable
{
    private readonly ClientWebSocket _webSocket;
    private readonly CancellationTokenSource _cts = new();
    private readonly Uri _uri;
    
    public bool IsConnected => _webSocket.State == WebSocketState.Open;
    public string Hostname => _uri.Host;
    public int Port => _uri.Port;
    
    public delegate Task MoonrakerClientConnectDisconnectEventHandler(MoonrakerClient sender);
    
    public event MoonrakerClientConnectDisconnectEventHandler OnConnect;
    public event MoonrakerClientConnectDisconnectEventHandler OnDisconnect;

    public MoonrakerClient(Uri uri)
    {
        _webSocket = new ClientWebSocket();
        _uri = uri;
    }

    public async Task Connect()
    {
        await _webSocket.ConnectAsync(_uri, _cts.Token);
        
        // Skip if not open
        if(_webSocket.State != WebSocketState.Open)
            return;
        
        Thread receiveLoop = new(StartReceiveLoop);
        receiveLoop.Start();
        OnConnect?.Invoke(this);
    }

    public async Task Disconnect()
    { 
        await _cts.CancelAsync();
        if(_webSocket.State != WebSocketState.Open)
            return;
        
        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "NormalClosure", CancellationToken.None);
        OnDisconnect?.Invoke(this);
    }
    
    public void Dispose()
    {
        _webSocket.Dispose();
    }

    private async void StartReceiveLoop()
    {
        try
        {
            byte[] buffer = new byte[1024];
            string stringBuffer = string.Empty;
            while (_webSocket.State == WebSocketState.Open)
            {
                var result= await _webSocket.ReceiveAsync(buffer, _cts.Token);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    break;
                }else if (result.MessageType == WebSocketMessageType.Text)
                {
                    stringBuffer += Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (!result.EndOfMessage)
                        continue;

                    var jsonRpc = JsonSerializer.Deserialize<JsonRpc>(stringBuffer);
                    if(jsonRpc == null)
                        throw new Exception("Unable to parse json rpc");

                    // StringBuffer is no longer needed
                    stringBuffer = string.Empty;
                
                    // Attempt full parse
                    if (TryReceiveWebsocketNotification(jsonRpc))
                    {
                        Console.WriteLine("Received websocket notification");
                    }
                }
            }

            OnDisconnect.Invoke(this);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    
}