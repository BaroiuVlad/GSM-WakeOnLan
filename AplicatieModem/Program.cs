using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AplicatieModem;
using AplicatieModem.Core;
using AplicatieModem.Data;
using AplicatieModem.Hardware;
using AplicatieModem.Models;
using AplicatieModem.Network;
using System.IO;
using System;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "GSM_WOL_Service";
});

string xmlPath = Path.Combine(AppContext.BaseDirectory, "users.xml");

string detectedPort = ModemDetector.FindModemPort(115200);

if (string.IsNullOrEmpty(detectedPort))
{
    throw new Exception("Modemul GSM nu a putut fi detectat pe niciun port COM.");
}

builder.Services.AddSingleton(new ServiceConfig
{
    ComPort = detectedPort,
    BaudRate = 115200,
    PollingIntervalMs = 2000
});

builder.Services.AddSingleton(new XmlConfigManager(xmlPath));
builder.Services.AddSingleton<AuthorizationService>();
builder.Services.AddSingleton<GsmModemController>();
builder.Services.AddSingleton<WakeOnLanClient>();
builder.Services.AddHostedService<Worker>();

try
{
    IHost host = builder.Build();
    host.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}