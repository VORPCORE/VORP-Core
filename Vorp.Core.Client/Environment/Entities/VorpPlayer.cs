using Vorp.Core.Client.Commands;
using Vorp.Core.Client.Interface;

namespace Vorp.Core.Client.Environment.Entities
{
    public class VorpPlayer : Entity
    {
        public PluginManager pluginManager => PluginManager.Instance;
        public ClientConfig clientConfig => ClientConfiguration.Config;

        private int _playerPedId;

        public int PlayerId { get; private set; }
        public int ServerId { get; private set; }

        Ped _pedCache;
        public Ped Ped
        {
            get
            {
                if (_pedCache == null)
                    _pedCache = new Ped(PlayerPedId);

                if (_pedCache.Handle != PlayerPedId)
                    _pedCache = new Ped(PlayerPedId);

                return _pedCache;
            }
        }

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
            RequestServerInformation();

            pluginManager.ClientGateway.Mount("vorp:user:group:client", new Action<string>(group =>
            {
                Group = group;
                Logger.Trace($"Group updated to: {Group}");

                foreach (KeyValuePair<CommandContext, List<Tuple<CommandInfo, Commands.ICommand>>> entry in pluginManager.CommandFramework.Registry)
                {
                    CommandContext commandContext = entry.Key;
                    List<Tuple<CommandInfo, Commands.ICommand>> tuples = entry.Value;

                    if (commandContext.IsRestricted && commandContext.RequiredRoles.Contains(Group))
                    {
                        foreach (Tuple<CommandInfo, Commands.ICommand> item in tuples)
                        {
                            BaseScript.TriggerEvent("chat:addSuggestion", $"/{commandContext.Aliases[0]} {item.Item1.Aliases[0]}", $"{item.Item1.Description}");
                        }
                    }
                    else
                    {
                        foreach (Tuple<CommandInfo, Commands.ICommand> item in tuples)
                        {
                            BaseScript.TriggerEvent("chat:addSuggestion", $"/{commandContext.Aliases[0]} {item.Item1.Aliases[0]}", $"{item.Item1.Description}");
                        }
                    }
                }
            }));
        }

        internal async Task Teleport(Vector3 pos)
        {
            await Screen.FadeOut(500);
            float groundZ = pos.Z;
            Vector3 norm = pos;

            IsPositionFrozen = true;

            if (API.GetGroundZAndNormalFor_3dCoord(pos.X, pos.Y, pos.Z, ref groundZ, ref norm))
                norm = new Vector3(pos.X, pos.Y, groundZ);

            Position = norm;
            IsPositionFrozen = false;

            await Screen.FadeIn(500);
        }

        async void RequestServerInformation()
        {
            string group = await pluginManager.ClientGateway.Get<string>("vorp:user:group", ServerId);
            Logger.Trace($"Server returned group '{group}'");
            Group = group;
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

        protected string _group { get; private set; }
        public string Group
        {
            set
            {
                _group = value;
            }
            get
            {
                return _group;
            }
        }

        public void EnableEagleeye(bool enable) => Function.Call((Hash)0xA63FCAD3A6FEC6D2, PlayerId, enable);
        public void EnableCustomDeadeyeAbility(bool enable) => Function.Call((Hash)0x95EE1DEE1DCD9070, PlayerId, enable);
    }
}
