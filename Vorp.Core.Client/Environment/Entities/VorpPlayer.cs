using Vorp.Core.Client.Commands;
using Vorp.Shared.Commands;

namespace Vorp.Core.Client.Environment.Entities
{
    public class VorpPlayer
    {
        public PluginManager pluginManager => PluginManager.Instance;
        public ClientConfig clientConfig => ClientConfiguration.Config;

        private int _playerPedId;

        public int PlayerId { get; private set; }
        public int ServerId { get; private set; }

        Ped _pedCache;
        public Ped Character
        {
            get
            {
                int handle = GetPlayerPed(PlayerId);

                if (ReferenceEquals(_pedCache, null) || handle != _pedCache.Handle)
                    _pedCache = new Ped(handle);

                return _pedCache;
            }
        }

        public virtual int PlayerPedId => _pedCache.Handle;

        public string PlayerName { get; private set; }

        public VorpPlayer(int playerId)
        {
            PlayerId = playerId;
            ServerId = GetPlayerServerId(playerId);
            PlayerName = GetPlayerName(playerId);
            Logger.Trace($"New Player Created: {playerId}: {PlayerName}");
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

        public async Task SetModel(string model)
        {
            uint hash = (uint)GetHashKey(model);
            if (!(Function.Call<bool>(Hash.IS_MODEL_IN_CDIMAGE, hash)))
            {
                Logger.Error($"Model is not loaded.");
                return;
            }

            if (Function.Call<bool>(Hash.IS_MODEL_VALID, hash))
            {
                Function.Call(Hash.REQUEST_MODEL, hash);
                while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, hash))
                {
                    await Common.MoveToMainThread();
                }
            }
            else
            {
                Debug.WriteLine($"Model {hash} is not valid!");
            }

            Function.Call((Hash)0xED40380076A31506, PlayerId(), hash, true);
        }

        async void RequestServerInformation()
        {
            string group = await pluginManager.ClientGateway.Get<string>("vorp:user:group", ServerId);
            Logger.Trace($"Server returned group '{group}'");
            Group = group;
        }

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
