using System;
using System.Collections.Generic;
using System.Text;

namespace AplicatieModem.Models
{
    public class ServiceConfig
    {

        public string ComPort { get; set; } = string.Empty;
        public int BaudRate { get; set; } = 115200;
        public int PollingIntervalMs { get; set; } = 2000;
    }
}
