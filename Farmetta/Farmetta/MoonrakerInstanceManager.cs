using Farmetta.Data;
using MoonrakerAPI;

namespace Farmetta;

public class MoonrakerInstanceManager
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Dictionary<int, MoonrakerClient> _moonrakerClients; // Lock on _moonrakerClients
    private readonly Dictionary<string, MoonrakerClient> _moonrakerClientsDictionary; // Lock on _moonrakerClientsDictionary
    
    
    private int _reconnectAttemptRateSeconds;// Lock on this

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
        _moonrakerClientsDictionary = new Dictionary<string, MoonrakerClient>();

        using var db = GetDbContext();

        foreach (var moonrakerClient in db.MoonrakerInstances)
        {
            int moonrakerClientId = moonrakerClient.Id;
            string moonrakerClientName = moonrakerClient.Name;
            Uri moonrakerUri = moonrakerClient.Uri;
            
            MoonrakerClient moonrakerClientInstance = new(moonrakerUri);
            _moonrakerClients.Add(moonrakerClientId, moonrakerClientInstance);
            _moonrakerClientsDictionary.Add(moonrakerClientName, moonrakerClientInstance);
        }
        
        // TODO: Start Thread to reconnect
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

    public MoonrakerClient this[string instanceName]
    {
        get
        {
            lock (_moonrakerClientsDictionary)
            {
                return _moonrakerClientsDictionary[instanceName];
            }
        }
    }

    public IReadOnlyList<string> GetAllMoonrakerInstanceNames()
    {
        lock (_moonrakerClientsDictionary)
        {
            return _moonrakerClientsDictionary.Keys.ToList();
        }
    }

    public List<MoonrakerClient> GetAllMoonrakerClients()
    {
        lock (_moonrakerClientsDictionary)
        {
            return _moonrakerClientsDictionary.Values.ToList();
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
            Uri = moonrakerInstanceUri
        };

        await using var db = GetDbContext();
        db.MoonrakerInstances.Add(connectionInfo);
        await db.SaveChangesAsync();

        MoonrakerClient moonrakerClient = new(moonrakerInstanceUri);

        lock (_moonrakerClients)
        {
            _moonrakerClients.Add(connectionInfo.Id, moonrakerClient);
        }

        lock (_moonrakerClientsDictionary)
        {
            _moonrakerClientsDictionary.Add(moonrakerInstanceName, moonrakerClient);
        }

        return moonrakerClient;
    }

    private ApplicationDbContext GetDbContext()
    {
        var scope = _scopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    private void AttemptReconnect()
    {
        while (true)
        {
            // TODO: Repeatedly attempt to reconnect if not connected
        }
        // ReSharper disable once FunctionNeverReturns
    }
    
}