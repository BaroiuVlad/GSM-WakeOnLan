using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using AplicatieModem.Models;
using AplicatieModem.Core;

namespace AplicatieModem.Data
{
    public class XmlConfigManager
    {
        private readonly string _filePath;

        public XmlConfigManager(string filePath)
        {
            _filePath = filePath;
        }

        public Dictionary<string, AuthorizedUser> LoadUsers()
        {
            var usersDictionary = new Dictionary<string, AuthorizedUser>();

            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine($"[EROARE] Fișierul XML nu există: {_filePath}");
                    return usersDictionary;
                }

                XDocument xmlDoc = XDocument.Load(_filePath);

                foreach (XElement element in xmlDoc.Descendants("User"))
                {
                    string? phone = element.Element("PhoneNumber")?.Value;

                    if (!string.IsNullOrWhiteSpace(phone))
                    {
                        string normalizedPhone =
                            AuthorizationService.NormalizePhoneNumber(phone);

                        usersDictionary[normalizedPhone] = new AuthorizedUser
                        {
                            Name = element.Element("Name")?.Value,
                            PhoneNumber = normalizedPhone,
                            MacAddress = element.Element("MacAddress")?.Value
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[XML ERROR] {ex.Message}");
            }

            return usersDictionary;
        }
    }
}