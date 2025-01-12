// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Nodes;
using MoonrakerAPI.API;
using MoonrakerAPI.WebsocketNotifications;

string json = "{\n    \"jsonrpc\": \"2.0\",\n    \"method\": \"notify_proc_stat_update\",\n    \"params\": [{\n        \"moonraker_stats\": {\n            \"time\": 1615837812.0894408,\n            \"cpu_usage\": 1.99,\n            \"memory\": 23636,\n            \"mem_units\": \"kB\"\n        },\n        \"cpu_temp\": 44.008,\n        \"network\": {\n            \"lo\": {\n                \"rx_bytes\": 114555457,\n                \"tx_bytes\": 114555457,\n                \"bandwidth\": 2911.49\n            },\n            \"wlan0\": {\n                \"rx_bytes\": 48773134,\n                \"tx_bytes\": 115035939,\n                \"bandwidth\": 3458.77\n            }\n        },\n        \"system_cpu_usage\": {\n            \"cpu\": 2.53,\n            \"cpu0\": 3.03,\n            \"cpu1\": 5.1,\n            \"cpu2\": 1.02,\n            \"cpu3\": 1\n        },\n        \"websocket_connections\": 2\n    }]\n}";

var test = JsonSerializer.Deserialize<JsonRpc>(json);

ProcessStatisticUpdate statsUpdate = new(test ?? throw new InvalidOperationException());