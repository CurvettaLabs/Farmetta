// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Nodes;
using MoonrakerAPI.API;
using MoonrakerAPI.WebsocketNotifications;

string json = "{\n    \"jsonrpc\": \"2.0\",\n    \"method\": \"notify_gcode_response\",\n    \"params\": [\"response message\"]\n}";
var test = JsonSerializer.Deserialize<JsonRpc>(json);

GcodeResponse statsUpdate = new(test ?? throw new InvalidOperationException());