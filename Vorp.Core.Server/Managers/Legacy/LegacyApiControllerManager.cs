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
        private LegacyCallbackManager _callbackManager => LegacyCallbackManager.GetModule();

        public override void Begin()
        {
            Logger.Info($"[MANAGER] Legacy API Controller Manager Init");

            Event("vorp:getCharacter", new Action<int, CallbackDelegate>(OnGetCharacter));
            ExportDictionary.Add("GetCharacter", new Func<int, Dictionary<string, dynamic>>(ExportGetCharacter));
            ExportDictionary.Add("GetUser", new Func<int, Dictionary<string, dynamic>>(ExportGetUser));

            Event("vorp:addMoney", new Action<int, int, double>(OnAddMoney));
            ExportDictionary.Add("AddMoney", new Func<int, int, double, Task<bool>>(ExportAddMoney));

            Event("vorp:removeMoney", new Action<int, int, double>(OnRemoveMoney));
            ExportDictionary.Add("RemoveMoney", new Func<int, int, double, Task<bool>>(ExportRemoveMoney));

            Event("vorp:addXp", new Action<int, int>(OnAddExperience));
            ExportDictionary.Add("AddExperience", new Func<int, int, Task<bool>>(ExportAddExperience));

            Event("vorp:removeXp", new Action<int, int>(OnRemoveExperience));
            ExportDictionary.Add("RemoveExperience", new Func<int, int, Task<bool>>(ExportRemoveExperience));

            Event("vorp:setJob", new Action<int, string>(OnSetJob));
            ExportDictionary.Add("SetJob", new Func<int, string, Task<bool>>(ExportSetJob));

            Event("vorp:setGroup", new Action<int, string>(OnSetCharacterGroup));
            ExportDictionary.Add("SetCharacterGroup", new Func<int, string, Task<bool>>(ExportSetCharacterGroup));

            // Review these events
            Event("vorp:saveLastCoords", new Action<Player, Vector3, float>(OnSaveLastCoords));
            Event("vorp:ImDead", new Action<Player, bool>(OnPlayerIsDead));

            Event("getCore", new Action<CallbackDelegate>(OnGetCore));
        }

        // Sadly no server side native for IsEntityDead yet.
        private async void OnPlayerIsDead([FromSource] Player player, bool isDead)
        {
            if (!UserSessions.ContainsKey(player.Handle)) return;
            User user = UserSessions[player.Handle];
            await user.ActiveCharacter.SetDead(isDead);
        }

        // Why does this even exist?! when saving a character on Drop, the position needs to be handled, maybe asking the client every minute?
        // If OneSync is enabled, we can just ask for this information on the server via player.Character
        private void OnSaveLastCoords([FromSource] Player player, Vector3 position, float heading)
        {
            if (!UserSessions.ContainsKey(player.Handle)) return;
            User user = UserSessions[player.Handle];

            JsonBuilder jb = new();
            jb.Add("x", position.X);
            jb.Add("y", position.Y);
            jb.Add("z", position.Z);
            jb.Add("heading", heading);

            user.ActiveCharacter.Coords = jb.Build();
        }

        private async void OnGetCore(CallbackDelegate cb)
        {
            while (!Instance.IsServerReady)
            {
                await BaseScript.Delay(100);
            }

            try
            {
                Logger.Warn($"Event 'getCore' is deprecated, please update your methods to use the exports.");
                Logger.Warn($"This will be removed in the future, and the version will be stated when this happens.");
                Dictionary<string, dynamic> Core = new Dictionary<string, dynamic>()
                {
                    { "getUser", new AuxDelegate(ExportGetUser) },
                    { "maxCharacters", ServerConfiguration.MaximumCharacters },
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
                    { "getUsers", new AuxGetConnectedUsers(GetConnectedUsers) },
                    { "addRpcCallback", new Action<string, CallbackDelegate>((name, cb) => {
                            if (LegacyCallbackManager.CallbackManagerInstance.Callbacks.ContainsKey(name)) return;
                            LegacyCallbackManager.CallbackManagerInstance.Callbacks.Add(name, cb);
                            Logger.Debug($"Added RPC Callback for {GetInvokingResource()}, Event '{name}'");
                        })
                    }
                };

                if (Core is not null)
                    cb.Invoke(Core);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"OnGetCore Exception: {GetInvokingResource()}");
            }
        }

        private Dictionary<string, Dictionary<string, dynamic>> GetConnectedUsers()
        {
            Dictionary<string, Dictionary<string, dynamic>> users = new Dictionary<string, Dictionary<string, dynamic>>();
            foreach (Player player in PlayersList)
            {
                if (!UserSessions.ContainsKey(player.Handle)) continue;
                string steamIdent = $"steam:{player.Identifiers["steam"]}";
                users.Add(steamIdent, UserSessions[player.Handle].GetUser());
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

        private Dictionary<string, dynamic> ExportGetUser(int serverId)
        {
            User user = GetUser(serverId);
            // return something that can be checked by the calling member
            if (user == null) return new Dictionary<string, dynamic>();
            return user.GetUser();
        }

        private User GetUser(int serverId)
        {
            Player player = PlayersList[serverId];
            if (player == null)
            {
                Logger.Error($"[LegacyApi] Player '{serverId}' not found.");
                return null;
            }

            string steamId = player.Identifiers["steam"];

            if (!UserSessions.ContainsKey(steamId))
            {
                Logger.Error($"[LegacyApi] Player [{steamId}] '{player.Name}' not found in Active Users.");
                return null;
            }
            return UserSessions[steamId];
        }
    }
}
