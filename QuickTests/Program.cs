// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Nodes;
using KlipperInstaller;
using MoonrakerAPI.API;
using MoonrakerAPI.WebsocketNotifications;

KlipperInstanceFactory factory = new KlipperInstanceFactory();

factory.UpdateDeps();
