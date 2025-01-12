using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

namespace MoonrakerAPI;

public partial class MoonrakerClient
{
    private readonly ClientWebSocket _webSocket;
    private readonly CancellationTokenSource _cts = new();
    private readonly Uri _uri;
    
    public event EventHandler<string> MessageReceived;

    public MoonrakerClient(Uri uri)
    {
        _webSocket = new ClientWebSocket();
        _uri = uri;
        
        
    }

    public async Task Connect()
    {
        await _webSocket.ConnectAsync(_uri, CancellationToken.None);
        Thread receiveLoop = new(StartReceiveLoop);
        receiveLoop.Start();
    }

    private async Task<string> AwaitStringMessage()
    {
        return "";
    }

    private async void StartReceiveLoop()
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

                try
                {
                    MessageReceived.Invoke(this, stringBuffer);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                stringBuffer = string.Empty;

            }
        }
    }
}