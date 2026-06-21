using System;
using System.IO.Ports;
using System.Threading;

namespace AplicatieModem.Hardware
{
    public static class ModemDetector
    {
        public static string FindModemPort(int baudRate)
        {
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                try
                {
                    using (SerialPort serialPort = new SerialPort(port, baudRate))
                    {
                        serialPort.ReadTimeout = 1000;
                        serialPort.WriteTimeout = 1000;
                        serialPort.Open();

                        serialPort.Write("AT\r");
                        Thread.Sleep(500);

                        string response = serialPort.ReadExisting();

                        if (response.Contains("OK"))
                        {
                            return port;
                        }
                    }
                }
                catch
                {
                }
            }

            return null;
        }
    }
}