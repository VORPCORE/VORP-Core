using System.Collections.Generic;
using System.Threading.Tasks;
using Vorp.Core.Server.Models;
using Vorp.Shared.Records;

namespace Vorp.Core.Server.Managers.Legacy
{
    public class LegacyUserManager : Manager<LegacyUserManager>
    {
        ServerConfig _srvCfg => ServerConfiguration.Config;

        public override void Begin()
        {
            Event("vorp:playerSpawn", new Action<Player>(OnPlayerSpawn));

            Event("vorp:getUser", new Action<int, CallbackDelegate>(OnGetUser));
            ExportDictionary.Add("GetUser", new Func<int, Dictionary<string, dynamic>>(ExportGetUser));

            Event("vorp:getCharacter", new Action<int, CallbackDelegate>(OnGetActiveCharacter));
            ExportDictionary.Add("GetActiveCharacter", new Func<int, Dictionary<string, dynamic>>(ExportActiveGetCharacter));

            // This event is not secure
            Event("vorp:addMoney", new Action<int, int, double>(OnAddMoneyAsync));
            ExportDictionary.Add("ExportAddCurrency", new Func<int, int, double, Task<bool>>(ExportAddCurrencyAsync));
            // This event is not secure
            Event("vorp:removeMoney", new Action<int, int, double>(OnRemoveMoneyAsync));
            ExportDictionary.Add("ExportRemoveCurrency", new Func<int, int, double, Task<bool>>(ExportRemoveCurrencyAsync));
            // This event is not secure
            Event("vorp:addXp", new Action<int, int>(OnAddExperienceAsync));
            ExportDictionary.Add("ExportAddExperience", new Func<int, int, Task<bool>>(ExportAddExperienceAsync));
            // This event is not secure
            Event("vorp:removeXp", new Action<int, int>(OnRemoveExperienceAsync));
            ExportDictionary.Add("ExportRemoveExperience", new Func<int, int, Task<bool>>(ExportRemoveExperienceAsync));
            // This event is not secure
            Event("vorp:setJob", new Action<int, string>(OnSetCharacterJobAsync));
            ExportDictionary.Add("ExportSetCharacterJob", new Func<int, string, Task<bool>>(ExportSetCharacterJobAsync));
            // This event is not secure
            Event("vorp:setGroup", new Action<int, string>(OnSetCharacterGroupAsync));
            ExportDictionary.Add("ExportSetCharacterGroup", new Func<int, string, Task<bool>>(ExportSetCharacterGroupAsync));
        }

        private async Task<bool> ExportSetCharacterGroupAsync(int serverId, string job)
        {
            User user = PluginManager.ToUser(serverId);
            if (user == null) return false;
            return await user.ActiveCharacter.SetGroup(job);
        }

        private async void OnSetCharacterGroupAsync(int serverId, string job)
        {
            Logger.Warn($"Event 'vorp:setJob' was invoked by '{GetInvokingResource()}', this is an unsecure event, it is recommended to use the export 'OnSetCharacterGroup'.");
            await ExportSetCharacterGroupAsync(serverId, job);
        }

        private async Task<bool> ExportSetCharacterJobAsync(int serverId, string job)
        {
            User user = PluginManager.ToUser(serverId);
            if (user == null) return false;
            return await user.ActiveCharacter.SetJob(job);
        }

        private async void OnSetCharacterJobAsync(int serverId, string job)
        {
            Logger.Warn($"Event 'vorp:setJob' was invoked by '{GetInvokingResource()}', this is an unsecure event, it is recommended to use the export 'ExportSetCharacterJob'.");
            await ExportSetCharacterJobAsync(serverId, job);
        }

        private async Task<bool> ExportRemoveExperienceAsync(int serverId, int amount)
        {
            User user = PluginManager.ToUser(serverId);
            if (user == null) return false;
            return await user.ActiveCharacter.AdjustExperience(false, amount);
        }

        private async void OnRemoveExperienceAsync(int serverId, int amount)
        {
            Logger.Warn($"Event 'vorp:removeXp' was invoked by '{GetInvokingResource()}', this is an unsecure event, it is recommended to use the export 'ExportRemoveExperience'.");
            await ExportRemoveExperienceAsync(serverId, amount);
        }

        private async Task<bool> ExportAddExperienceAsync(int serverId, int amount)
        {
            User user = PluginManager.ToUser(serverId);
            if (user == null) return false;
            return await user.ActiveCharacter.AdjustExperience(true, amount);
        }

        private async void OnAddExperienceAsync(int serverId, int amount)
        {
            Logger.Warn($"Event 'vorp:addXp' was invoked by '{GetInvokingResource()}', this is an unsecure event, it is recommended to use the export 'ExportAddExperience'.");
            await ExportAddExperienceAsync(serverId, amount);
        }

        private async Task<bool> ExportRemoveCurrencyAsync(int serverId, int currencyType, double amount)
        {
            User user = PluginManager.ToUser(serverId);
            if (user == null) return false;
            return await user.ActiveCharacter.AdjustCurrency(false, currencyType, amount);
        }

        private async void OnRemoveMoneyAsync(int serverId, int currencyType, double amount)
        {
            Logger.Warn($"Event 'vorp:removeMoney' was invoked by '{GetInvokingResource()}', this is an unsecure event, it is recommended to use the export 'ExportRemoveCurrency'.");
            await ExportRemoveCurrencyAsync(serverId, currencyType, amount);
        }

        private async Task<bool> ExportAddCurrencyAsync(int serverId, int currencyType, double amount)
        {
            User user = PluginManager.ToUser(serverId);
            if (user == null) return false;
            return await user.ActiveCharacter.AdjustCurrency(true, currencyType, amount);
        }

        private async void OnAddMoneyAsync(int serverId, int currencyType, double amount)
        {
            Logger.Warn($"Event 'vorp:addMoney' was invoked by '{GetInvokingResource()}', this is an unsecure event, it is recommended to use the export 'ExportAddCurrency'.");
            await ExportAddCurrencyAsync(serverId, currencyType, amount);
        }

        private Dictionary<string, dynamic> ExportActiveGetCharacter(int serverId)
        {
            User user = PluginManager.ToUser(serverId);
            if (user == null) return null;
            return user.GetActiveCharacter();
        }

        private void OnGetActiveCharacter(int serverId, CallbackDelegate cb)
        {
            cb.Invoke(ExportActiveGetCharacter(serverId));
        }

        private Dictionary<string, dynamic> ExportGetUser(int serverId)
        {
            User user = PluginManager.ToUser(serverId);
            if (user is null)
            {
                Logger.CriticalError($"Cannot find user for serverId '{serverId}'");
            }
            return user.GetUser();
        }

        private void OnGetUser(int serverId, CallbackDelegate cb)
        {
            cb.Invoke(ExportGetUser(serverId));
        }

        // TODO: REplace with internal methods
        private void OnPlayerSpawn([FromSource] Player player)
        {
            int playerHandle = int.Parse(player.Handle);
            if (!UserSessions.ContainsKey(playerHandle))
            {
                Logger.Error($"Player '{player.Handle}' tried to spawn, but isn't setup.");
                return;
            }

            User user = UserSessions[playerHandle];
            int numberOfCharacters = user.NumberOfCharacters;
            if (numberOfCharacters == 0)
            {
                player.TriggerEvent("vorp_CreateNewCharacter");
                return;
            }

            if (_srvCfg.UserConfig.Characters.Maximum == 1 && numberOfCharacters <= 1)
            {
                player.TriggerEvent("vorp_SpawnUniqueCharacter");
                return;
            }

            player.TriggerEvent("vorp_GoToSelectionMenu");
        }
    }
}
