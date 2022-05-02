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

        public override void Begin()
        {
            Logger.Info($"[MANAGER] Legacy API Controller Manager Init");

            Event("vorp:getCharacter", new Action<int, CallbackDelegate>(OnGetCharacter));
            ExportDictionary.Add("GetCharacter", new Func<int, Dictionary<string, dynamic>>(ExportGetCharacter));
            ExportDictionary.Add("GetUser", new Func<int, Dictionary<string, dynamic>>(ExportGetUser));

            Event("vorp:addMoney", new Action<int, int, double>(OnAddMoneyAsync));
            ExportDictionary.Add("AddMoney", new Func<int, int, double, Task<bool>>(ExportAddMoneyAsync));

            Event("vorp:removeMoney", new Action<int, int, double>(OnRemoveMoneyAsync));
            ExportDictionary.Add("RemoveMoney", new Func<int, int, double, Task<bool>>(ExportRemoveMoneyAsync));

            Event("vorp:addXp", new Action<int, int>(OnAddExperienceAsync));
            ExportDictionary.Add("AddExperience", new Func<int, int, Task<bool>>(ExportAddExperienceAsync));

            Event("vorp:removeXp", new Action<int, int>(OnRemoveExperienceAsync));
            ExportDictionary.Add("RemoveExperience", new Func<int, int, Task<bool>>(ExportRemoveExperienceAsync));

            Event("vorp:setJob", new Action<int, string>(OnSetJobAsync));
            ExportDictionary.Add("SetJob", new Func<int, string, Task<bool>>(ExportSetJobAsync));

            Event("vorp:setGroup", new Action<int, string>(OnSetCharacterGroupAsync));
            ExportDictionary.Add("SetCharacterGroup", new Func<int, string, Task<bool>>(ExportSetCharacterGroupAsync));

            // Review these events
            Event("vorp:saveLastCoords", new Action<Player, Vector3, float>(OnSaveLastCoords));
            Event("vorp:ImDead", new Action<Player, bool>(OnPlayerIsDeadAsync));

            Event("getCore", new Action<CallbackDelegate>(OnGetCoreAsync));

            // New Exports
            ExportDictionary.Add("SetActiveCharacter", new Func<int, int, bool>(ExportSetActiveCharacter));
        }

        // Sadly no server side native for IsEntityDead yet.
        private async void OnPlayerIsDeadAsync([FromSource] Player player, bool isDead)
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

        private async void OnGetCoreAsync(CallbackDelegate cb)
        {
            while (!Instance.IsServerReady)
            {
                await BaseScript.Delay(100);
            }

            try
            {
                // PENDING WARNING WHEN SET TO DO CHANGES, WHY, BECAUSE THIS IS HORRIBLE!
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
                                    Logger.Trace(msg);
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

        private async Task<bool> ExportSetCharacterGroupAsync(int serverId, string group)
        {
            User user = PluginManager.ToUser(serverId);
            if (user == null) return false;
            return await user.ActiveCharacter.SetGroup(group);
        }

        private async void OnSetCharacterGroupAsync(int serverId, string group)
        {
            await ExportSetJobAsync(serverId, group);
        }

        private async Task<bool> ExportSetJobAsync(int serverId, string job)
        {
            User user = PluginManager.ToUser(serverId);
            if (user == null) return false;
            return await user.ActiveCharacter.SetJob(job);
        }

        private async void OnSetJobAsync(int serverId, string job)
        {
            await ExportSetJobAsync(serverId, job);
        }

        private async Task<bool> ExportRemoveExperienceAsync(int serverId, int experience)
        {
            User user = PluginManager.ToUser(serverId);
            if (user == null) return false;
            bool result = await user.ActiveCharacter.AdjustExperience(false, experience);
            return result;
        }

        private async void OnRemoveExperienceAsync(int serverId, int experience)
        {
            await ExportRemoveExperienceAsync(serverId, experience);
        }

        private async Task<bool> ExportAddExperienceAsync(int serverId, int experience)
        {
            User user = PluginManager.ToUser(serverId);
            if (user == null) return false;
            bool result = await user.ActiveCharacter.AdjustExperience(true, experience);
            return result;
        }

        private async void OnAddExperienceAsync(int serverId, int experience)
        {
            await ExportAddExperienceAsync(serverId, experience);
        }

        private async Task<bool> ExportRemoveMoneyAsync(int serverId, int currency, double amount)
        {
            User user = PluginManager.ToUser(serverId);
            if (user == null) return false;
            bool result = await user.ActiveCharacter.AdjustCurrency(false, currency, amount);
            return result;
        }

        private async void OnRemoveMoneyAsync(int serverId, int currency, double amount)
        {
            await ExportRemoveMoneyAsync(serverId, currency, amount);
        }

        private async Task<bool> ExportAddMoneyAsync(int serverId, int currency, double amount)
        {
            User user = PluginManager.ToUser(serverId);
            if (user == null) return false;
            bool result = await user.ActiveCharacter.AdjustCurrency(true, currency, amount);
            return result;
        }

        private async void OnAddMoneyAsync(int serverId, int currency, double amount)
        {
            await ExportAddMoneyAsync(serverId, currency, amount);
        }

        private void OnGetCharacter(int serverId, CallbackDelegate cb)
        {
            cb.Invoke(ExportGetCharacter(serverId));
        }

        private Dictionary<string, dynamic> ExportGetCharacter(int serverId)
        {
            User user = PluginManager.ToUser(serverId);
            // return something that can be checked by the calling member
            if (user == null) return new Dictionary<string, dynamic>();
            return user.GetActiveCharacter();
        }

        private Dictionary<string, dynamic> ExportGetUser(int serverId)
        {
            User user = PluginManager.ToUser(serverId);
            // return something that can be checked by the calling member
            if (user == null) return new Dictionary<string, dynamic>();
            return user.GetUser();
        }

        private bool ExportSetActiveCharacter(int serverId, int characterId)
        {
            User user = PluginManager.ToUser(serverId);
            return user.SetActiveCharacter(characterId);
        }
    }
}
