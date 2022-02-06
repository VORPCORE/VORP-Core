using System.Threading.Tasks;
using Vorp.Core.Client.Managers;
using Vorp.Core.Client.RedM.Enums;

namespace Vorp.Core.Client.Environment.Entities
{
    class VorpPlayer
    {
        public PluginManager pluginManager => PluginManager.Instance;
        public ClientConfigManager configManager => ClientConfigManager.GetModule();

        public int PlayerId { get; private set; }
        public int ServerId { get; private set; }
        public int PlayerPedId { get; private set; }
        public string PlayerName { get; private set; }

        public VorpPlayer(int playerId, int playerPedId)
        {
            PlayerId = playerId;
            ServerId = GetPlayerServerId(playerId);
            PlayerPedId = playerPedId;
            PlayerName = GetPlayerName(playerId);
        }
    }
}
