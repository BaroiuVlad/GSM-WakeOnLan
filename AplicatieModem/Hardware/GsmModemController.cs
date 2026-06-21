using System;
using System.IO.Ports;
using System.Threading;
using AplicatieModem.Models;

namespace AplicatieModem.Hardware
{
    public class GsmModemController
    {
        private readonly SerialPort _serialPort;

        public GsmModemController(ServiceConfig config)
        {
            _serialPort = new SerialPort(config.ComPort, config.BaudRate)
            {
                ReadTimeout = 3000,
                WriteTimeout = 3000,
                DtrEnable = true,
                RtsEnable = true
            };
        }

        public void OpenConnection()
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                    Console.WriteLine("[MODEM] Conectat.");

                    _serialPort.WriteLine("AT\r");
                    Thread.Sleep(300);

                    _serialPort.WriteLine("AT+CMGF=1\r");
                    Thread.Sleep(300);

                    _serialPort.WriteLine("AT+CLIP=1\r");
                    Thread.Sleep(300);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EROARE MODEM] {ex.Message}");
            }
        }

        public void CloseConnection()
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                    Console.WriteLine("[MODEM] Conexiune inchisa.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EROARE CLOSE] {ex.Message}");
            }
        }

        public (string PhoneNumber, string ActivityType, string MessageBody)? CheckForActivity()
        {
            try
            {
                if (!_serialPort.IsOpen)
                    return null;

                string bufferData = _serialPort.ReadExisting();

                if (bufferData.Contains("+CLIP:"))
                {
                    _serialPort.WriteLine("ATH\r");

                    string[] lines = bufferData.Split(
                        new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.RemoveEmptyEntries);

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("+CLIP:"))
                        {
                            string[] parts = line.Split(',');
                            if (parts.Length > 0)
                            {
                                string phoneNumber = parts[0].Replace("+CLIP:", "").Replace("\"", "").Trim();
                                return (phoneNumber, "APEL", "");
                            }
                        }
                    }
                }

                _serialPort.WriteLine("AT+CMGL=\"REC UNREAD\"\r");
                Thread.Sleep(1000);

                string smsResponse = _serialPort.ReadExisting();

                if (smsResponse.Contains("+CMGL:"))
                {
                    string[] lines = smsResponse.Split(
                        new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].StartsWith("+CMGL:"))
                        {
                            string[] headerParts = lines[i].Split(',');

                            if (headerParts.Length >= 3)
                            {
                                string indexPart = lines[i].Split(':')[1].Split(',')[0].Trim();
                                string phoneNumber = headerParts[2].Replace("\"", "");
                                string messageBody = "";

                                if (i + 1 < lines.Length)
                                {
                                    messageBody = lines[i + 1].Trim();
                                }

                                _serialPort.WriteLine($"AT+CMGD={indexPart}\r");
                                Console.WriteLine($"[SMS STERS] Index {indexPart}");

                                return (phoneNumber, "SMS", messageBody);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EROARE ACTIVITATE] {ex.Message}");
            }

            return null;
        }
    }
}