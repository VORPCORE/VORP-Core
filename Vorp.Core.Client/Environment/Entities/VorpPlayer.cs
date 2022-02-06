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
        public void EnableEagleeye(bool enable) => Function.Call((Hash)0xA63FCAD3A6FEC6D2, PlayerId, enable);
        public void EnableCustomDeadeyeAbility(bool enable) => Function.Call((Hash)0x95EE1DEE1DCD9070, PlayerId, enable);
    }
}
