#if SERVER
using Dapper;
using Vorp.Core.Server.Database;
using Vorp.Core.Server.Managers;
#endif

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Vorp.Shared.Models;
using System.Threading.Tasks;

namespace Vorp.Shared.Records
{
    public record User
    {
#if SERVER
        const string SQL_UPDATE_GROUP = "update users set `group` = @group where `identifier` = @identifier;";
        const string SQL_UPDATE_WARNING = "update users set `warnings` = @warningCount where `identifier` = @identifier;";
        const string SQL_GET_CHARACTERS = "select * from characters where `identifier` = @identifier";

        ServerConfigManager _serverConfigManager => ServerConfigManager.GetModule();
#endif

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

#if SERVER
        public Player Player { get; private set; }
        public string Endpoint => GetPlayerEndpoint(ServerId);
#endif
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
            return await DapperDatabase<bool>.ExecuteAsync("INSERT INTO users VALUES (@identifier, @group, @warnings, @banned);", dynamicParameters);
        }

        // Server must be the location to change these settings, client only needs to read it
        public bool SetActiveCharacter(int charId)
        {
            if (!Characters.ContainsKey(charId)) return false;
            Characters.ToList().ForEach(x => x.Value.IsActive = false);
            Characters[charId].IsActive = true;
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
            if (changePersisted)
                Warnings = warnings;
            return changePersisted;
        }

        public async Task GetCharacters()
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("identifier", SteamIdentifier);
            List<Character> characters = await DapperDatabase<Character>.GetListAsync(SQL_GET_CHARACTERS, dynamicParameters);
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
                ["source"] = ServerId,
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
                ["getUsedCharacter"] = ActiveCharacter,
                ["getUserCharacters"] = Characters,
                ["getNumOfCharacters"] = Characters.Count,
                ["addCharacter"] = new Func<string, string, string, string, Task<bool>>(async (firstname, lastname, skin, comps) =>
                {
                    if (Characters.Count >= _serverConfigManager.UserConfig.Characters.Maximum) return false;
                    try
                    {
                        // AddCharacter(firstname, lastname, skin, comps);
                        return true;
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
                        // DeleteCharacter(charid);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"removeCharacter: {e.Message}");
                        return false;
                    }

                }),
                ["setUsedCharacter"] = new Func<int, Task<bool>>(async (charid) =>
                {
                    try
                    {
                        SetActiveCharacter(charid);
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

        internal void UpdateServerId(string handle)
        {
            ServerId = handle;
        }

        public Dictionary<string, dynamic> GetActiveCharacter()
        {
            Character activeChar = ActiveCharacter;

            return new Dictionary<string, dynamic>()
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
                { "setJobGrade", new Action<int>(jobGrade => activeChar.JobGrade = jobGrade) },
                { "setGroup", new Action<string>(async group => await activeChar.SetGroup(group)) },
                { "setMoney", new Action<double>(cash => activeChar.SetCash(cash)) },
                { "setGold", new Action<double>(gold => activeChar.SetGold(gold)) },
                { "setRol", new Action<double>(roleToken => activeChar.SetRoleToken(roleToken)) },
                { "setXp", new Action<int>(experience => activeChar.SetExperience(experience)) },
                { "setFirstname", new Action<string>(firstname => Logger.Error($"Method 'setFirstname' Deprecated, please inform us if you us this, and why.")) },
                { "setLastname", new Action<string>(firstlastnamename => Logger.Error($"Method 'setLastname' Deprecated, please inform us if you us this, and why.")) },
                { "updateSkin", new Action<string>(skin => activeChar.Skin = skin) },
                { "updateComps", new Action<string>(comp => activeChar.Components = comp) },
                { "addCurrency", new Action<int, double>(async (currency, amount) =>
                    {
                        await activeChar.AdjustCurrency(true, currency, amount);
                        UpdateUI();
                    })
                },
                { "removeCurrency", new Action<int, double>(async (currency, amount) =>
                    {
                        await activeChar.AdjustCurrency(false, currency, amount);
                        UpdateUI();
                    })
                },
                { "addXp", new Action<int>(async experience =>
                    {
                        await activeChar.AdjustExperience(true, experience);
                        UpdateUI();
                    })
                },
                { "removeXp", new Action<int>(async experience =>
                    {
                        await activeChar.AdjustExperience(false, experience);
                        UpdateUI();
                    })
                },
            };
        }

        public void UpdateUI()
        {
            Character activeChar = ActiveCharacter;
            JsonBuilder jb = new JsonBuilder();
            jb.Add("type", "ui");
            jb.Add("action", "update");
            jb.Add("moneyquanty", activeChar.Cash);
            jb.Add("goldquanty", activeChar.Gold);
            jb.Add("rolquanty", activeChar.RoleToken);
            jb.Add("xp", activeChar.Experience);
            jb.Add("serverId", ServerId);
            Player.TriggerEvent("vorp:updateUi", $"{jb}");
        }
#endif
    }
}
