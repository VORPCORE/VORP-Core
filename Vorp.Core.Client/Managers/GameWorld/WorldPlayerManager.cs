using System.Collections.Generic;
using System.Threading.Tasks;
using Vorp.Core.Client.Environment.Entities;

namespace Vorp.Core.Client.Managers.GameWorld
{
    public class WorldPlayerManager : Manager<WorldPlayerManager>
    {
        ClientConfigManager ClientConfigManager => ClientConfigManager.GetModule();
        Dictionary<int, WorldPlayer> playersInScope = new Dictionary<int, WorldPlayer>();
        float ConfigDistance => ClientConfigManager.PlayerNameConfig.Distance;

        public override void Begin()
        {

        }

        private async Task OnWorldPlayer()
        {
            for (int activePlayerId = 0; activePlayerId < GetActivePlayers(); activePlayerId++)
            {
                if (playersInScope.ContainsKey(activePlayerId)) continue;
                if (GetPlayerPed(activePlayerId) == PlayerPedId()) continue;

                if (NetworkIsPlayerActive(activePlayerId))
                {
                    Vector3 playerCoords = GetEntityCoords(PlayerPedId(), false, false);
                    Vector3 targetCoords = GetEntityCoords(activePlayerId, false, false);
                    if (VorpAPI.Distance(playerCoords, targetCoords) < ConfigDistance)
                    {
                        int playerPedId = GetPlayerPed(activePlayerId);
                        playersInScope[activePlayerId] = new WorldPlayer(activePlayerId, playerPedId);
                    }
                    else
                    {
                        DisposePlayer(activePlayerId);
                    }
                }
                else
                {
                    DisposePlayer(activePlayerId);
                }
            }
        }

        void DisposePlayer(int activePlayerId)
        {
            if (playersInScope.ContainsKey(activePlayerId))
                playersInScope[activePlayerId].Dispose();
        }
    }
}
