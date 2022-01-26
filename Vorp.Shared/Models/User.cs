using System;
using System.Collections.Generic;
using System.Text;

namespace Vorp.Shared.Models
{
    public class User
    {
        public string ServerId { get; private set; }

        // DB Keys
        public string SteamIdentifier { get; private set; }
        public string LicenseIdentifier { get; private set; }

        // Administration
        public string Group { get; private set; }
        public int Warnings { get; private set; }

        // Character Items
        public Dictionary<int, Character> Characters { get; private set; } = new Dictionary<int, Character>();
        public int NumberOfCharacters => Characters.Count;

        public User(string serverId,
                    string steamId,
                    string license,
                    string group,
                    int warnings)
        {
            ServerId = serverId;
            SteamIdentifier = steamId;
            LicenseIdentifier = license;
            Group = group;
            Warnings = warnings;
        }

        // Methods
    }
}
