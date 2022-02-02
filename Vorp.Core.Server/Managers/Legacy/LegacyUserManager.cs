using System.Collections.Generic;
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
        }

        private Dictionary<string, dynamic> ExportGetUser(int serverId)
        {
            string srvId = $"{serverId}";

            if (!ActiveUsers.ContainsKey(srvId))
            {
                return null;
            }

            User user = ActiveUsers[srvId];
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
    }
}
