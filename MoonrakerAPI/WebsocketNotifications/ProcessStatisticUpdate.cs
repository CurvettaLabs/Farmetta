using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using MoonrakerAPI.API;

namespace MoonrakerAPI.WebsocketNotifications;

public class ProcessStatisticUpdate :
    JsonRpc
{
    public const string MethodName = "notify_proc_stat_update";
    
    public ProcessStatisticUpdate(JsonRpc baseJsonRpc)
    {
        JsonRpcVersion = baseJsonRpc.JsonRpcVersion;
        Method = baseJsonRpc.Method;
        
        if(Method != MethodName)
            throw new ArgumentException($"Method must be \"{MethodName}\"");

        /*if(RawParams == null)
            throw new ArgumentException("RawParams must not be null");
            */
        
        var paramsResult = JsonSerializer.Deserialize<List<ProcessStatisticUpdateParams>>(baseJsonRpc.RawParams.ToString());
        Params = paramsResult ?? throw new ArgumentException("Unable to parse ProcessStatisticUpdateParams");
    }
    
    [JsonPropertyName("params")]
    public List<ProcessStatisticUpdateParams> Params { get; set; }
}

public class MoonrakerStats

{
    [JsonPropertyName("time")]
    public double Time { get; set; }
    
    [JsonPropertyName("cpu_usage")]
    public double CpuUsage { get; set; }
    
    [JsonPropertyName("memory")]
    public int Memory { get; set; }
    
    [JsonPropertyName("mem_units")]
    public string MemUnits { get; set; }
}

public class NetworkStats
{
    [JsonPropertyName("rx_bytes")]
    public long RxBytes { get; set; }
    
    [JsonPropertyName("tx_bytes")]
    public long TxBytes { get; set; }
    
    [JsonPropertyName("bandwidth")]
    public double Bandwidth { get; set; }
}


public class ProcessStatisticUpdateParams
{
    [JsonPropertyName("moonraker_stats")]
    public MoonrakerStats MoonrakerStats { get; set; }
    
    [JsonPropertyName("cpu_temp")]
    public double CpuTemp { get; set; }
    
    [JsonPropertyName("network")]
    public Dictionary<string, NetworkStats> Network { get; set; } // Changed to Dictionary
    
    [JsonPropertyName("system_cpu_usage")]
    public Dictionary<string, double> SystemCpuUsage { get; set; }
    
    [JsonPropertyName("websocket_connections")]
    public int WebsocketConnections { get; set; }
}