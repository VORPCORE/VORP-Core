#if SERVER
using Dapper;
using Vorp.Core.Server.Database;
using Vorp.Core.Server;
using Vorp.Core.Server.Models;
using System.Threading.Tasks;
using Vorp.Shared.Models;
#endif

using Lusive.Events.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Vorp.Shared.Records
{
    [Serialization]
    public partial class User
    {
#if SERVER
        const string SQL_UPDATE_GROUP = "update users set `group` = @group where `identifier` = @identifier;";
        const string SQL_UPDATE_WARNING = "update users set `warnings` = @warningCount where `identifier` = @identifier;";
        const string SQL_GET_CHARACTERS = "select * from characters where `identifier` = @identifier;";

        ServerConfig _serverConfigManager => ServerConfiguration.Config;
#endif

        #region Fields

        [Description("identifier")]
        public string SteamIdentifier { get; private set; }
        public string Name { get; private set; }
        [Description("group")]
        public string Group { get; private set; }
        [Description("warnings")]
        public int Warnings { get; private set; }
        [Description("banned")]
        public bool Banned { get; private set; }

        #endregion

        public User(string identifier, string group, int warnings, sbyte banned)
        {
            SteamIdentifier = identifier;
            Group = group;
            Warnings = warnings;
            Banned = banned == 1;
        }

        public User(string identifier, string name, string group, int warnings, sbyte banned)
        {
            SteamIdentifier = identifier;
            Group = group;
            Warnings = warnings;
            Banned = banned == 1;
        }

        public User(string cfxServerHandle,
                    string name,
                    string steamId,
                    string license,
                    string group,
                    int warnings)
        {
            CFXServerID = int.Parse(cfxServerHandle);
            Name = name;
            SteamIdentifier = steamId;
            LicenseIdentifier = license;
            Group = group;
            Warnings = warnings;
        }

#if SERVER
        [JsonIgnore] [Ignore] public Player Player { get; private set; }

        public void AddPlayer(Player player) => Player = player;
        public void SetName(string name) => Name = name;

        [JsonIgnore] public string Endpoint => GetPlayerEndpoint($"{CFXServerID}");
#endif
        [JsonIgnore] public int CFXServerID { get; private set; } = 0;
        // DB Keys
        [JsonIgnore] public ulong DiscordIdentifier { get; private set; }
        [JsonIgnore] public string LicenseIdentifier { get; private set; }

        // Character Items
        public Dictionary<int, Character> Characters { get; private set; } = new Dictionary<int, Character>();
        public int NumberOfCharacters => Characters.Count;
        public Character ActiveCharacter => Characters.Select(x => x.Value).Where(x => x.IsActive).FirstOrDefault();

#if SERVER
        public long GameTimeWhenDropped { get; private set; }
        public bool IsActive { get; internal set; }

        public void MarkPlayerHasDropped()
        {
            GameTimeWhenDropped = GetGameTimer();
            IsActive = false;
        }

        internal async Task<bool> Save()
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("identifier", SteamIdentifier);
            dynamicParameters.Add("group", Group);
            dynamicParameters.Add("warnings", Warnings);
            dynamicParameters.Add("banned", Banned);

            string query = @"INSERT INTO users VALUES (@identifier, @group, @warnings, @banned)
                             ON DUPLICATE KEY UPDATE `group` = @group, `warnings` = @warnings, `banned` = @banned;";

            return await DapperDatabase<bool>.ExecuteAsync(query, dynamicParameters);
        }

        // Server must be the location to change these settings, client only needs to read it
        public async Task<bool> SetActiveCharacterAsync(int charId)
        {
            if (!Characters.ContainsKey(charId)) return false;

            Logger.Info($"Setting Character '{charId}' active for '{Name}' with ServerID '{CFXServerID}'.");

            Characters.ToList().ForEach(x => x.Value.IsActive = false);

            Character character = Characters[charId];
            character.IsActive = true;

            Logger.Info($"Character '{character.Fullname}' is now active for '{Name}' with ServerID '{CFXServerID}'.");

            // TODO: Replace this method with something, better.
            Player.TriggerEvent("vorp:SelectedCharacter", charId);
            await BaseScript.Delay(100);
            BaseScript.TriggerEvent("vorp:SelectedCharacter", CFXServerID, GetActiveCharacter());

            return true;
        }

        // should review group to become a bit array of permissions?
        public async Task<bool> SetGroup(string group, bool internalOnly = false)
        {
            if (internalOnly)
            {
                Group = group;
                return true;
            }

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("identifier", SteamIdentifier);
            dynamicParameters.Add("group", group);
            bool changePersisted = await DapperDatabase<User>.ExecuteAsync(SQL_UPDATE_GROUP, dynamicParameters);
            await Common.MoveToMainThread();
            if (changePersisted)
                Group = group;
            return changePersisted;
        }

        public async Task<bool> SetWarning(int warnings)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("identifier", SteamIdentifier);
            dynamicParameters.Add("warningCount", Warnings);
            bool changePersisted = await DapperDatabase<User>.ExecuteAsync(SQL_UPDATE_WARNING, dynamicParameters);
            await Common.MoveToMainThread();
            if (changePersisted)
                Warnings = warnings;
            return changePersisted;
        }

        public async Task GetCharacters()
        {
            Logger.Debug($"Requesting characters for '{SteamIdentifier}' '{Name}'");

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("identifier", SteamIdentifier);
            List<Character> characters = await DapperDatabase<Character>.GetListAsync(SQL_GET_CHARACTERS, dynamicParameters);

            await Common.MoveToMainThread();

            foreach (Character character in characters)
                Characters.Add(character.CharacterId, character);
        }

        public Dictionary<string, dynamic> GetUser()
        {
            return new Dictionary<string, dynamic>()
            {
                ["getIdentifier"] = SteamIdentifier,
                ["getGroup"] = Group,
                ["getPlayerwarnings"] = Warnings,
                ["source"] = CFXServerID,
                ["setGroup"] = new Func<string, Task<bool>>(async (group) =>
                {
                    try
                    {
                        return await SetGroup(group);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"setGroup: {e.Message}");
                        return false;
                    }

                }),
                ["setPlayerWarnings"] = new Func<int, Task<bool>>(async (warnings) =>
                {
                    try
                    {
                        return await SetWarning(warnings);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"setPlayerWarnings: {e.Message}");
                        return false;
                    }
                }),
                ["getUsedCharacter"] = GetActiveCharacter(),
                ["getUserCharacters"] = Characters,
                ["getNumOfCharacters"] = Characters.Count,
                ["addCharacter"] = new Func<string, string, string, string, Task<bool>>(async (firstname, lastname, skin, comps) =>
                {
                    if (Characters.Count >= _serverConfigManager.UserConfig.Characters.Maximum) return false;
                    try
                    {
                        Character character = new Character();
                        character.Firstname = firstname;
                        character.Lastname = lastname;
                        character.Skin = skin;
                        character.Components = comps;
                        return await character.Save();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"addCharacter: {e.Message}");
                        return false;
                    }
                }),
                ["removeCharacter"] = new Func<int, Task<bool>>(async (charid) =>
                {
                    try
                    {
                        int activeCharacterId = ActiveCharacter.CharacterId;
                        bool result = await ActiveCharacter.Delete();

                        // if successful, remove the character from the servers memory
                        if (result)
                            Characters.Remove(activeCharacterId);

                        return result;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"removeCharacter: {e.Message}");
                        return false;
                    }

                }),
                ["setUsedCharacter"] = new Func<int, bool>((charid) =>
                {
                    try
                    {
                        SetActiveCharacterAsync(charid);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"setUsedCharacter: {e.Message}");
                        return false;
                    }
                })
            };
        }

        internal void UpdateServerId(int handle)
        {
            CFXServerID = handle;
        }

        public Dictionary<string, dynamic> GetActiveCharacter()
        {
            Character activeChar = ActiveCharacter;

            if (activeChar == null)
            {
                Logger.Error($"[{GetInvokingResource()}] Player '{Player.Name}' does not current have an active character.");
                return null;
            }

            Dictionary<string, dynamic> characterDict = new()
            {
                { "identifier", activeChar.SteamIdentifier },
                { "charIdentifier", activeChar.CharacterId },
                { "group", activeChar.Group },
                { "job", activeChar.Job },
                { "jobGrade", activeChar.JobGrade },
                { "money", activeChar.Cash },
                { "gold", activeChar.Gold },
                { "rol", activeChar.RoleToken },
                { "xp", activeChar.Experience },
                { "firstname", activeChar.Firstname },
                { "lastname", activeChar.Lastname },
                { "inventory", activeChar.Inventory },
                { "status", activeChar.Status },
                { "coords", activeChar.Coords },
                { "isdead", activeChar.IsDead },
                { "skin", activeChar.Skin },
                { "comps", activeChar.Components },
                { "setStatus", new Action<string>(status => activeChar.Status = status) },
                { "setJob", new Action<string>(async job => await activeChar.SetJob(job)) },
                { "setJobGrade", new Action<int>(async jobGrade => await activeChar.SetJobGrade(jobGrade)) },
                { "setJobAndGrade", new Action<string, int>(async (job, jobGrade) => await activeChar.SetJobAndGrade(job, jobGrade)) },
                { "setGroup", new Action<string>(async group => await activeChar.SetGroup(group)) },
                { "setMoney", new Action<double>(cash => activeChar.SetCashAsync(cash)) },
                { "setGold", new Action<double>(gold => activeChar.SetGoldAsync(gold)) },
                { "setRol", new Action<double>(roleToken => activeChar.SetRoleTokenAsync(roleToken)) },
                { "setXp", new Action<int>(experience => activeChar.SetExperienceAsync(experience)) },
                { "setFirstname", new Action<string>(firstname => activeChar.SetFirstnameAsync(firstname)) },
                { "setLastname", new Action<string>(lastname => activeChar.SetLastnameAsync(lastname)) },
                { "updateSkin", new Action<string>(skin => activeChar.Skin = skin) },
                { "updateComps", new Action<string>(comp => activeChar.Components = comp) },
                { "addCurrency", new Action<int, double>(async (currency, amount) =>
                    {
                        await activeChar.AdjustCurrency(true, currency, amount);
                    })
                },
                { "removeCurrency", new Action<int, double>(async (currency, amount) =>
                    {
                        await activeChar.AdjustCurrency(false, currency, amount);
                    })
                },
                { "addXp", new Action<int>(async experience =>
                    {
                        await activeChar.AdjustExperience(true, experience);
                    })
                },
                { "removeXp", new Action<int>(async experience =>
                    {
                        await activeChar.AdjustExperience(false, experience);
                    })
                },
            };

            Logger.Trace($"Requested Active Character '{characterDict["charIdentifier"]}' with name '{activeChar.Fullname}'.");

            return characterDict;
        }
#endif

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
