using Farmetta.Data;
using MoonrakerAPI;

namespace Farmetta;

public class MoonrakerInstanceManager : IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    private int _reconnectAttemptRateSeconds;// Lock on this
    private Dictionary<int, MoonrakerClient> _moonrakerClients;

    public delegate void MoonrakerEvent(string clientName, MoonrakerClient moonrakerClient);
    
    public event MoonrakerEvent? OnMoonrakerClientConnected;
    
    public event MoonrakerEvent? OnMoonrakerClientDisconnected;
    
    public event MoonrakerEvent? OnMoonrakerClientCreated;
    public event MoonrakerEvent? OnMoonrakerClientRemoved;

    public int ReconnectAttemptRateSeconds 
    {
        get
        {
            lock (this)
            {
                return _reconnectAttemptRateSeconds;
            }
        }
        set
        {
            lock (this)
            {
                _reconnectAttemptRateSeconds = value;
            }
        }
    }


    public MoonrakerInstanceManager(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _moonrakerClients = new Dictionary<int, MoonrakerClient>();

        using var scope = GetServiceScope();
        using var db = GetDbContext(scope);

        foreach (var moonrakerClient in db.MoonrakerInstances)
        {
            int moonrakerClientId = moonrakerClient.Id;
            Uri moonrakerUri = moonrakerClient.Uri;
            bool shouldKeepConnectionOpen = moonrakerClient.ShouldReconnect;
            
            MoonrakerClient moonrakerClientInstance = new(moonrakerUri);
            _moonrakerClients.Add(moonrakerClientId, moonrakerClientInstance);

            moonrakerClientInstance.OnDisconnect += OnClientDisconnect;

            if (shouldKeepConnectionOpen)
                _ = moonrakerClientInstance.Connect();
        }
    }

    public MoonrakerClient this[int instanceId]
    {
        get
        {
            lock (_moonrakerClients)
            {
                return _moonrakerClients[instanceId];
            }
        }
    }

    public void Dispose()
    {
        foreach (var moonrakerClient in _moonrakerClients)
            moonrakerClient.Value.Dispose();
    }

    public MoonrakerClient? this[string instanceName]
    {
        get
        {
            using var scope = GetServiceScope();
            using var db = GetDbContext(scope);

            var instanceConnectionInfo = db.MoonrakerInstances.FirstOrDefault(instance => instance.Name == instanceName);
            if (instanceConnectionInfo == null)
                return null;
            
            lock (_moonrakerClients)
            {
                return _moonrakerClients[instanceConnectionInfo.Id];
            }
        }
    }

    public IReadOnlyList<string> GetAllMoonrakerInstanceNames()
    {
        using var scope = GetServiceScope();
        using var db = GetDbContext(scope);
        return db.MoonrakerInstances.Select(instance => instance.Name).ToList();
    }

    public IReadOnlyList<MoonrakerClient> GetAllMoonrakerClients()
    {
        lock (_moonrakerClients)
        {
            return _moonrakerClients.Values.ToList();
        }
    }

    /**
     * Creates a new MoonrakerClient.
     * If AutoReconnect is on, a connection will be established soon after.
     */
    public async Task<MoonrakerClient> CreateMoonrakerClient(string moonrakerInstanceName, Uri moonrakerInstanceUri)
    {
        MoonrakerInstanceConnectionInfo connectionInfo = new()
        {
            Id = 0,
            Name = moonrakerInstanceName,
            Uri = moonrakerInstanceUri,
            ShouldReconnect = true
        };

        using var scope = GetServiceScope();
        await using var db = GetDbContext(scope);
        db.MoonrakerInstances.Add(connectionInfo);
        await db.SaveChangesAsync();

        MoonrakerClient moonrakerClient = new(moonrakerInstanceUri);

        lock (_moonrakerClients)
        {
            _moonrakerClients.Add(connectionInfo.Id, moonrakerClient);
        }
        
        moonrakerClient.OnDisconnect += OnClientDisconnect;

        await moonrakerClient.Connect();
        
        OnMoonrakerClientCreated?.Invoke(moonrakerInstanceName, moonrakerClient);

        return moonrakerClient;
    }

    public async Task RemoveClient(string moonrakerInstanceName)
    {
        using var scope = GetServiceScope();
        await using var db = GetDbContext(scope);
        
        var connectionInfo = db.MoonrakerInstances.FirstOrDefault(instance => instance.Name == moonrakerInstanceName);
        if(connectionInfo == null)
            return;

        db.MoonrakerInstances.Remove(connectionInfo);
        await db.SaveChangesAsync();
        
        MoonrakerClient moonrakerClient = this[connectionInfo.Id];
        await moonrakerClient.Disconnect();
        
        lock (_moonrakerClients)
        {
            _moonrakerClients.Remove(connectionInfo.Id);
        }
        
        OnMoonrakerClientRemoved?.Invoke(moonrakerInstanceName, moonrakerClient);

    }

    private IServiceScope GetServiceScope()
    {
        return _scopeFactory.CreateScope();
    }

    private ApplicationDbContext GetDbContext(IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    private async Task OnClientDisconnect(MoonrakerClient moonrakerClient)
    {
        int clientId;
        lock (_moonrakerClients)
        {
            clientId = _moonrakerClients.First(client => client.Value == moonrakerClient).Key;
        }
        
        using var scope = GetServiceScope();
        await using var db = GetDbContext(scope);
        
        var connectionInfo = db.MoonrakerInstances.FirstOrDefault(instance => instance.Id == clientId);
        if(connectionInfo == null)
            return;
        
        string clientName = connectionInfo.Name;
        
        OnMoonrakerClientDisconnected?.Invoke(clientName, moonrakerClient);

        if (connectionInfo.ShouldReconnect)
        {
            while (!moonrakerClient.IsConnected)
            {
                await Task.Delay(TimeSpan.FromSeconds(_reconnectAttemptRateSeconds));
            
                await moonrakerClient.Connect();
                OnMoonrakerClientConnected?.Invoke(clientName, moonrakerClient);
            }
        }
    }
    
    
}