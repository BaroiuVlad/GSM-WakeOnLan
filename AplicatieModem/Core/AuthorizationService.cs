using System;
using AplicatieModem.Data;

namespace AplicatieModem.Core
{
    public class AuthorizationService
    {
        private readonly XmlConfigManager _configManager;

        public AuthorizationService(XmlConfigManager configManager)
        {
            _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
        }

        public string? CheckAuthorization(string incomingPhoneNumber)
        {
            if (string.IsNullOrWhiteSpace(incomingPhoneNumber))
            {
                return null;
            }

            string cleanIncomingNumber = NormalizePhoneNumber(incomingPhoneNumber);

            var authorizedUsers = _configManager.LoadUsers();

            if (authorizedUsers.TryGetValue(cleanIncomingNumber, out var user))
            {
                return user.MacAddress;
            }

            return null;
        }

        public static string NormalizePhoneNumber(string phoneNumber)
        {
            string normalized = phoneNumber.Trim().Replace(" ", "").Replace("-", "");

            if (normalized.StartsWith("+40"))
                return "0" + normalized.Substring(3);

            if (normalized.StartsWith("0040"))
                return "0" + normalized.Substring(4);

            if (normalized.StartsWith("40") && normalized.Length == 11)
                return "0" + normalized.Substring(2);

            return normalized;
        }
    }
}