using Vorp.Core.Client.Managers;

namespace Vorp.Core.Client.Environment.Entities
{
    public class VorpPlayer : Entity
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
                {
                    Handle = PlayerPedId();
                    _playerPedId = PlayerPedId();
                }
                return _playerPedId;
            }
            private set
            {
                _playerPedId = value;
            }
        }

        public string PlayerName { get; private set; }

        public VorpPlayer(int playerId, int playerPedId) : base(playerPedId)
        {
            PlayerId = playerId;
            ServerId = GetPlayerServerId(playerId);
            PlayerPedId = playerPedId;
            PlayerName = GetPlayerName(playerId);
            Logger.Trace($"New Player Created: {playerId}/{playerPedId}: {PlayerName}");
        }

        public bool IsPositionFrozen
        {
            get => Function.Call<bool>((Hash)0x083D497D57B7400F, PlayerPedId);
            set => FreezeEntityPosition(PlayerPedId, value);
        }

        public bool IsCollisionEnabled
        {
            get => !GetEntityCollisionDisabled(PlayerPedId);
            set => SetEntityCollision(PlayerPedId, value, value);
        }

        public bool CanRagdoll
        {
            get => CanPedRagdoll(PlayerPedId);
            set => SetPedCanRagdoll(PlayerPedId, value);
        }

        public bool IsVisible
        {
            get => IsEntityVisible(PlayerPedId);
            set => SetEntityVisible(PlayerPedId, value);
        }

        public int Opacity
        {
            get => GetEntityAlpha(PlayerPedId);
            set => SetEntityAlpha(PlayerPedId, value, false);
        }

        public bool IsDead => IsEntityDead(PlayerPedId);
        public void EnableEagleeye(bool enable) => Function.Call((Hash)0xA63FCAD3A6FEC6D2, PlayerId, enable);
        public void EnableCustomDeadeyeAbility(bool enable) => Function.Call((Hash)0x95EE1DEE1DCD9070, PlayerId, enable);
    }
}
