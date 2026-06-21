using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace AplicatieModem.Network
{
    public class WakeOnLanClient
    {
        public void SendMagicPacket(string macAddress)
        {
            try
            {
                byte[] macBytes = ParseMacAddress(macAddress);

                byte[] packet = new byte[6 + (16 * 6)];

                for (int i = 0; i < 6; i++)
                    packet[i] = 0xFF;

                for (int i = 0; i < 16; i++)
                    Buffer.BlockCopy(macBytes, 0, packet, 6 + i * 6, 6);

                var localEndpoint = new IPEndPoint(IPAddress.Parse("10.0.30.124"), 0);
                using UdpClient client = new UdpClient(localEndpoint);
                client.EnableBroadcast = true;

                var endpoint = new IPEndPoint(IPAddress.Parse("10.0.30.255"), 9);
                client.Send(packet, packet.Length, endpoint);

                Console.WriteLine($"[WOL] Trimis către {macAddress}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WOL ERROR] {ex.Message}");
            }
        }

        private byte[] ParseMacAddress(string macAddress)
        {
            string cleanMac = macAddress
                .Replace(":", "")
                .Replace("-", "");

            if (cleanMac.Length != 12)
                throw new Exception("MAC invalid");

            return Enumerable.Range(0, 6)
                .Select(i => Convert.ToByte(cleanMac.Substring(i * 2, 2), 16))
                .ToArray();
        }
    }
}