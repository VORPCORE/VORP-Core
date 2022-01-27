using Dapper;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Vorp.Core.Server.Database;

namespace Vorp.Shared.Records
{
    public record User
    {
        #region Fields

        [Description("identifier")]
        public string SteamIdentifier { get; private set; }

        [Description("group")]
        public string Group { get; private set; }

        [Description("warnings")]
        public int Warnings { get; private set; }

        [Description("banned")]
        public bool Banned { get; private set; }

        #endregion


        public string ServerId { get; private set; }
        // DB Keys
        public ulong DiscordIdentifier { get; private set; }
        public string LicenseIdentifier { get; private set; }

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

#if SERVER
        // Server must be the location to change these settings, client only needs to read it
        public bool SetActiveCharacter(int charId)
        {
            if (!Characters.ContainsKey(charId)) return false;
            Characters.ToList().ForEach(x => x.Value.IsActive = false);
            Characters[charId].IsActive = true;
            return true;
        }

        // should review group to become a bit array of permissions?
        public void SetGroup(string group)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("id", SteamIdentifier);
            dynamicParameters.Add("group", group);
            bool changePersisted = DapperDatabase<User>.Execute("update users set `group` = @group where `identifier` = @id;", dynamicParameters);
            if (changePersisted)
                Group = group;
        }

        public void SetWarning(int warnings)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("id", SteamIdentifier);
            dynamicParameters.Add("warningCount", Warnings);
            bool changePersisted = DapperDatabase<User>.Execute("update users set `warnings` = @warningCount where `identifier` = @id;", dynamicParameters);
            if (changePersisted)
                Warnings = warnings;
        }

        public void GetCharacters()
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("id", SteamIdentifier);
            List<Character> characters = DapperDatabase<Character>.GetList("select * from characters where `identifier` = @id", dynamicParameters);
            foreach (Character character in characters)
                Characters.Add(character.CharacterId, character);
        }
#endif
    }
}
