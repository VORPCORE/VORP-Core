using Vorp.Core.Client.Managers;

namespace Vorp.Core.Client.Environment.Entities
{
    public class VorpPlayer
    {
        public PluginManager pluginManager => PluginManager.Instance;
        public ClientConfigManager configManager => ClientConfigManager.GetModule();

        private int _playerPedId;

        public int PlayerId { get; private set; }
        public int ServerId { get; private set; }
        public virtual int PlayerPedId
        {
            get
            {
                if (_playerPedId != PlayerPedId())
                    _playerPedId = PlayerPedId();
                return _playerPedId;
            }
            private set
            {
                _playerPedId = value;
            }
        }

        public string PlayerName { get; private set; }

        public VorpPlayer(int playerId, int playerPedId)
        {
            PlayerId = playerId;
            ServerId = GetPlayerServerId(playerId);
            PlayerPedId = playerPedId;
            PlayerName = GetPlayerName(playerId);
        }

        public Vector3 Position => GetEntityCoords(PlayerPedId, false, false);
        public float Heading => GetEntityHeading(PlayerPedId);
        public bool IsDead => IsEntityDead(PlayerPedId);
    }
}
