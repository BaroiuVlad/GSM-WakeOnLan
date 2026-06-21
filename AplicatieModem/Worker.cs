using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using AplicatieModem.Core;
using AplicatieModem.Hardware;
using AplicatieModem.Network;

namespace AplicatieModem
{
    public class Worker : BackgroundService
    {
        private readonly AuthorizationService _authService;
        private readonly GsmModemController _modemController;
        private readonly WakeOnLanClient _wolClient;

        public Worker(
            AuthorizationService authService,
            GsmModemController modemController,
            WakeOnLanClient wolClient)
        {
            _authService = authService;
            _modemController = modemController;
            _wolClient = wolClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _modemController.OpenConnection();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var incomingData = _modemController.CheckForActivity();

                    if (incomingData.HasValue)
                    {
                        if (incomingData.Value.ActivityType == "APEL")
                        {
                            Console.WriteLine($"\n[INFO] Apel primit de la: {incomingData.Value.PhoneNumber}");
                        }
                        else
                        {
                            Console.WriteLine($"\n[INFO] SMS primit de la: {incomingData.Value.PhoneNumber}");
                            Console.WriteLine($"[MESAJ] {incomingData.Value.MessageBody}");
                        }

                        string? targetMac = _authService.CheckAuthorization(incomingData.Value.PhoneNumber);

                        if (!string.IsNullOrWhiteSpace(targetMac))
                        {
                            Console.WriteLine($"[AUTORIZAT] Trimit WOL catre: {targetMac}");
                            _wolClient.SendMagicPacket(targetMac);
                        }
                        else
                        {
                            Console.WriteLine($"[RESPINS] Numar neautorizat.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WORKER ERROR] {ex.Message}");
                }

                await Task.Delay(2000, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _modemController.CloseConnection();

            return base.StopAsync(cancellationToken);
        }
    }
}