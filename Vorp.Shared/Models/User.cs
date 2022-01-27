using System.Collections.Generic;
using System.Linq;

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
        public Character ActiveCharacter => Characters.Select(x => x.Value).Where(x => x.IsActive).FirstOrDefault();

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

        public void SetActiveCharacter(int charId)
        {
            if (!Characters.ContainsKey(charId)) return;
            Characters.ToList().ForEach(x => x.Value.IsActive = false);
            Characters[charId].IsActive = true;
        }


        // should review group to become a bit array of permissions
        public void SetGroup(string group)
        {
            Group = group;
        }
    }
}
