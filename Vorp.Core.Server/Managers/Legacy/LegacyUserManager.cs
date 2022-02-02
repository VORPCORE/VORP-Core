using System.Collections.Generic;
using System.Threading.Tasks;
using Vorp.Shared.Records;

namespace Vorp.Core.Server.Managers.Legacy
{
    public class LegacyUserManager : Manager<LegacyUserManager>
    {
        ServerConfigManager _srvCfg => ServerConfigManager.GetModule();

        public override void Begin()
        {
            EventRegistry.Add("vorp:playerSpawn", new Action<Player>(OnPlayerSpawn));

            EventRegistry.Add("vorp:getUser", new Action<int, CallbackDelegate>(OnGetUser));
            ExportDictionary.Add("GetUser", new Func<int, Dictionary<string, dynamic>>(ExportGetUser));

            EventRegistry.Add("vorp:getCharacter", new Action<int, CallbackDelegate>(OnGetActiveCharacter));
            ExportDictionary.Add("GetActiveCharacter", new Func<int, Dictionary<string, dynamic>>(ExportActiveGetCharacter));
            
            // This event is not secure
            EventRegistry.Add("vorp:addMoney", new Action<int, int, double>(OnAddMoney));
            ExportDictionary.Add("ExportAddCurrency", new Func<int, int, double, Task<bool>>(ExportAddCurrency));
            // This event is not secure
            EventRegistry.Add("vorp:removeMoney", new Action<int, int, double>(OnRemoveMoney));
            ExportDictionary.Add("ExportRemoveCurrency", new Func<int, int, double, Task<bool>>(ExportRemoveCurrency));
            // This event is not secure
            EventRegistry.Add("vorp:addXp", new Action<int, int>(OnAddExperience));
            ExportDictionary.Add("ExportAddExperience", new Func<int, int, Task<bool>>(ExportAddExperience));
            // This event is not secure
            EventRegistry.Add("vorp:removeXp", new Action<int, int>(OnRemoveExperience));
            ExportDictionary.Add("ExportRemoveExperience", new Func<int, int, Task<bool>>(ExportRemoveExperience));
            // This event is not secure
            EventRegistry.Add("vorp:setJob", new Action<int, string>(OnSetCharacterJob));
            ExportDictionary.Add("ExportSetJob", new Func<int, string, Task<bool>>(ExportSetCharacterJob));
        }

        private async Task<bool> ExportSetCharacterJob(int serverId, string job)
        {
            User user = GetUser(serverId);
            if (user == null) return false;
            return await user.ActiveCharacter.SetJob(job);
        }

        private async void OnSetCharacterJob(int serverId, string job)
        {
            Logger.Warn($"Event 'vorp:setJob' was invoked by '{GetInvokingResource()}', this is an unsecure event, it is recommended to use the export 'ExportSetCharacterJob'.");
            await ExportSetCharacterJob(serverId, job);
        }

        private async Task<bool> ExportRemoveExperience(int serverId, int amount)
        {
            User user = GetUser(serverId);
            if (user == null) return false;
            return await user.ActiveCharacter.AdjustExperience(false, amount);
        }

        private async void OnRemoveExperience(int serverId, int amount)
        {
            Logger.Warn($"Event 'vorp:removeXp' was invoked by '{GetInvokingResource()}', this is an unsecure event, it is recommended to use the export 'ExportRemoveExperience'.");
            await ExportRemoveExperience(serverId, amount);
        }

        private async Task<bool> ExportAddExperience(int serverId, int amount)
        {
            User user = GetUser(serverId);
            if (user == null) return false;
            return await user.ActiveCharacter.AdjustExperience(true, amount);
        }

        private async void OnAddExperience(int serverId, int amount)
        {
            Logger.Warn($"Event 'vorp:addXp' was invoked by '{GetInvokingResource()}', this is an unsecure event, it is recommended to use the export 'ExportAddExperience'.");
            await ExportAddExperience(serverId, amount);
        }

        private async Task<bool> ExportRemoveCurrency(int serverId, int currencyType, double amount)
        {
            User user = GetUser(serverId);
            if (user == null) return false;
            return await user.ActiveCharacter.AdjustCurrency(false, currencyType, amount);
        }

        private async void OnRemoveMoney(int serverId, int currencyType, double amount)
        {
            Logger.Warn($"Event 'vorp:removeMoney' was invoked by '{GetInvokingResource()}', this is an unsecure event, it is recommended to use the export 'ExportRemoveCurrency'.");
            await ExportRemoveCurrency(serverId, currencyType, amount);
        }

        private async Task<bool> ExportAddCurrency(int serverId, int currencyType, double amount)
        {
            User user = GetUser(serverId);
            if (user == null) return false;
            return await user.ActiveCharacter.AdjustCurrency(true, currencyType, amount);
        }

        private async void OnAddMoney(int serverId, int currencyType, double amount)
        {
            Logger.Warn($"Event 'vorp:addMoney' was invoked by '{GetInvokingResource()}', this is an unsecure event, it is recommended to use the export 'ExportAddCurrency'.");
            await ExportAddCurrency(serverId, currencyType, amount);
        }

        private Dictionary<string, dynamic> ExportActiveGetCharacter(int serverId)
        {
            User user = GetUser(serverId);
            if (user == null) return null;
            return user.GetActiveCharacter();
        }

        private void OnGetActiveCharacter(int serverId, CallbackDelegate cb)
        {
            cb.Invoke(ExportActiveGetCharacter(serverId));
        }

        private Dictionary<string, dynamic> ExportGetUser(int serverId)
        {
            User user = GetUser(serverId);
            return user.GetUser();
        }

        private void OnGetUser(int serverId, CallbackDelegate cb)
        {
            cb.Invoke(ExportGetUser(serverId));
        }

        private void OnPlayerSpawn([FromSource] Player player)
        {
            if (!ActiveUsers.ContainsKey(player.Handle))
            {
                Logger.Error($"Player '{player.Handle}' tried to spawn, but isn't setup.");
                return;
            }

            User user = ActiveUsers[player.Handle];
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

        private User GetUser(int serverId)
        {
            string srvId = $"{serverId}";

            if (!ActiveUsers.ContainsKey(srvId))
            {
                return null;
            }

            return ActiveUsers[srvId];
        }
    }
}
