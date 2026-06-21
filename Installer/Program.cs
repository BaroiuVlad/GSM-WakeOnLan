using System;
using WixSharp;

namespace Installer
{
    class Program
    {
        static void Main()
        {
            string publishFolder = @"c:\practica_baroiu\aplicatiemodem (1)\aplicatiemodem\aplicatiemodem\bin\release\net10.0\publish";

            var project = new Project("GSM_WOL_Service",
                new Dir(@"%ProgramFiles%\GsmWolApp",
                    // Am adaugat explicit WixSharp.Files si WixSharp.File
                    new WixSharp.Files($@"{publishFolder}\*.*", file => !file.EndsWith("AplicatieModem.exe")),
                    new WixSharp.File($@"{publishFolder}\AplicatieModem.exe",
                        new ServiceInstaller
                        {
                            Name = "GSM_WOL_Service",
                            StartOn = SvcEvent.Install,
                            StopOn = SvcEvent.InstallUninstall_Wait,
                            RemoveOn = SvcEvent.Uninstall_Wait,
                            Type = SvcType.ownProcess,
                            Start = SvcStartType.auto
                        }
                    )
                )
            );

            project.GUID = new Guid("A3F45E67-1B2C-4D89-9A0B-1234567890AB");
            project.OutFileName = "GSM_WOL_Installer";

            Compiler.BuildMsi(project);
        }
    }
}