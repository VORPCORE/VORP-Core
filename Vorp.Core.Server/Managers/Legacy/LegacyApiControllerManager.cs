using System.Collections.Generic;
using System.Threading.Tasks;
using Vorp.Shared.Models;
using Vorp.Shared.Records;

namespace Vorp.Core.Server.Managers.Legacy
{
    public class LegacyApiControllerManager : Manager<LegacyApiControllerManager>
    {
        public delegate Dictionary<string, dynamic> AuxDelegate(int serverId);
        public delegate Dictionary<string, Dictionary<string, dynamic>> AuxGetConnectedUsers();

        private ServerConfigManager _serverConfigManager => ServerConfigManager.GetModule();

        public override void Begin()
        {
            Logger.Info($"[MANAGER] Legacy API Controller Manager Init");

            Instance.EventRegistry.Add("vorp:getCharacter", new Action<int, CallbackDelegate>(OnGetCharacter));
            Instance.ExportDictionary.Add("GetCharacter", new Func<int, Dictionary<string, dynamic>>(ExportGetCharacter));

            Instance.EventRegistry.Add("vorp:addMoney", new Action<int, int, double>(OnAddMoney));
            Instance.ExportDictionary.Add("AddMoney", new Func<int, int, double, Task<bool>>(ExportAddMoney));

            Instance.EventRegistry.Add("vorp:removeMoney", new Action<int, int, double>(OnRemoveMoney));
            Instance.ExportDictionary.Add("RemoveMoney", new Func<int, int, double, Task<bool>>(ExportRemoveMoney));

            Instance.EventRegistry.Add("vorp:addXp", new Action<int, int>(OnAddExperience));
            Instance.ExportDictionary.Add("AddExperience", new Func<int, int, Task<bool>>(ExportAddExperience));

            Instance.EventRegistry.Add("vorp:removeXp", new Action<int, int>(OnRemoveExperience));
            Instance.ExportDictionary.Add("RemoveExperience", new Func<int, int, Task<bool>>(ExportRemoveExperience));

            Instance.EventRegistry.Add("vorp:setJob", new Action<int, string>(OnSetJob));
            Instance.ExportDictionary.Add("SetJob", new Func<int, string, Task<bool>>(ExportSetJob));

            Instance.EventRegistry.Add("vorp:setGroup", new Action<int, string>(OnSetCharacterGroup));
            Instance.ExportDictionary.Add("SetCharacterGroup", new Func<int, string, Task<bool>>(ExportSetCharacterGroup));

            // Review these events
            Instance.EventRegistry.Add("vorp:saveLastCoords", new Action<Player, Vector3, float>(OnSaveLastCoords));
            Instance.EventRegistry.Add("vorp:ImDead", new Action<Player, bool>(OnPlayerIsDead));

            Instance.EventRegistry.Add("getCore", new Action<CallbackDelegate>(OnGetCore));
        }

        // Sadly no server side native for IsEntityDead yet.
        private async void OnPlayerIsDead([FromSource] Player player, bool isDead)
        {
            if (!ActiveUsers.ContainsKey(player.Handle)) return;
            User user = ActiveUsers[player.Handle];
            await user.ActiveCharacter.SetDead(isDead);
        }

        // Why does this even exist?! when saving a character on Drop, the position needs to be handled, maybe asking the client every minute?
        // If OneSync is enabled, we can just ask for this information on the server via player.Character
        private void OnSaveLastCoords([FromSource] Player player, Vector3 position, float heading)
        {
            if (!ActiveUsers.ContainsKey(player.Handle)) return;
            User user = ActiveUsers[player.Handle];

            JsonBuilder jb = new();
            jb.Add("x", position.X);
            jb.Add("y", position.Y);
            jb.Add("z", position.Z);
            jb.Add("heading", heading);

            user.ActiveCharacter.Coords = jb.Build();
        }

        private void OnGetCore(CallbackDelegate cb)
        {
            Dictionary<string, dynamic> core = new Dictionary<string, dynamic>()
            {
                { "getUser", new AuxDelegate(ExportGetCharacter) },
                { "maxCharacters", _serverConfigManager.UserConfig.Characters.Maximum },
                { "sendLog", new Action<string, string>((msg, type) =>
                    {
                        switch (type)
                        {
                            case "error":
                                Logger.Error(msg);
                                break;
                            case "warn":
                                Logger.Warn(msg);
                                break;
                            case "info":
                                Logger.Info(msg);
                                break;
                            case "success":
                                Logger.Success(msg);
                                break;
                            default:
                                Logger.Error($"Log Type '{type}' is unknown");
                                break;
                        }
                    })
                },
                { "getUsers", new AuxGetConnectedUsers(GetConnectedUsers) }
            };
            cb.Invoke(core);
        }

        private Dictionary<string, Dictionary<string, dynamic>> GetConnectedUsers()
        {
            Dictionary<string, Dictionary<string, dynamic>> users = new Dictionary<string, Dictionary<string, dynamic>>();
            foreach(Player player in PlayersList)
            {
                if (!ActiveUsers.ContainsKey(player.Handle)) continue;
                string steamIdent = $"steam:{player.Identifiers["steam"]}";
                users.Add(steamIdent, ActiveUsers[player.Handle].GetUser());
            }
            return users;
        }

        private async Task<bool> ExportSetCharacterGroup(int serverId, string group)
        {
            User user = GetUser(serverId);
            if (user == null) return false;
            return await user.ActiveCharacter.SetGroup(group);
        }

        private async void OnSetCharacterGroup(int serverId, string group)
        {
            await ExportSetJob(serverId, group);
        }

        private async Task<bool> ExportSetJob(int serverId, string job)
        {
            User user = GetUser(serverId);
            if (user == null) return false;
            return await user.ActiveCharacter.SetJob(job);
        }

        private async void OnSetJob(int serverId, string job)
        {
            await ExportSetJob(serverId, job);
        }

        private async Task<bool> ExportRemoveExperience(int serverId, int experience)
        {
            User user = GetUser(serverId);
            if (user == null) return false;
            bool result = await user.ActiveCharacter.AdjustExperience(false, experience);
            if (result)
                user.UpdateUI();
            return result;
        }

        private async void OnRemoveExperience(int serverId, int experience)
        {
            await ExportRemoveExperience(serverId, experience);
        }

        private async Task<bool> ExportAddExperience(int serverId, int experience)
        {
            User user = GetUser(serverId);
            if (user == null) return false;
            bool result = await user.ActiveCharacter.AdjustExperience(true, experience);
            if (result)
                user.UpdateUI();
            return result;
        }

        private async void OnAddExperience(int serverId, int experience)
        {
            await ExportAddExperience(serverId, experience);
        }

        private async Task<bool> ExportRemoveMoney(int serverId, int currency, double amount)
        {
            User user = GetUser(serverId);
            if (user == null) return false;
            bool result = await user.ActiveCharacter.AdjustCurrency(false, currency, amount);
            if (result)
                user.UpdateUI();
            return result;
        }

        private async void OnRemoveMoney(int serverId, int currency, double amount)
        {
            await ExportRemoveMoney(serverId, currency, amount);
        }

        private async Task<bool> ExportAddMoney(int serverId, int currency, double amount)
        {
            User user = GetUser(serverId);
            if (user == null) return false;
            bool result = await user.ActiveCharacter.AdjustCurrency(true, currency, amount);
            if (result)
                user.UpdateUI();
            return result;
        }

        private async void OnAddMoney(int serverId, int currency, double amount)
        {
            await ExportAddMoney(serverId, currency, amount);
        }

        private void OnGetCharacter(int serverId, CallbackDelegate cb)
        {
            cb.Invoke(ExportGetCharacter(serverId));
        }

        private Dictionary<string, dynamic> ExportGetCharacter(int serverId)
        {
            User user = GetUser(serverId);
            // return something that can be checked by the calling member
            if (user == null) return new Dictionary<string, dynamic>();
            return user.GetActiveCharacter();
        }

        private User GetUser(int serverId)
        {

            Player player = PlayersList[serverId];
            if (player == null)
            {
                Logger.Error($"[LegacyApi] Player not found.");
                return null;
            }
            if (!ActiveUsers.ContainsKey(player.Handle))
            {
                Logger.Error($"[LegacyApi] Player not found in Active Users.");
                return null;
            }
            return ActiveUsers[player.Handle];
        }
    }
}
