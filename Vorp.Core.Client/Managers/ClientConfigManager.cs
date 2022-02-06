using System;
using Vorp.Core.Client.Environment;
using Vorp.Core.Client.Environment.Config;

namespace Vorp.Core.Client.Managers
{
    public class ClientConfigManager : Manager<ClientConfigManager>
    {
        ClientConfig _configCache;

        public override void Begin()
        {

        }

        private ClientConfig GetConfig()
        {
            ClientConfig config = new();

            try
            {
                if (_configCache is not null)
                    return _configCache;

                _configCache = JsonConvert.DeserializeObject<ClientConfig>(Properties.Resources.clientConfig);
                return _configCache;
            }
            catch (Exception ex)
            {
                Logger.Error($"Config JSON File Exception\nDetails: {ex.Message}\nStackTrace:\n{ex.StackTrace}");
            }

            return config;
        }

        public PlayerNames PlayerNameConfig => GetConfig().PlayerNames;
        public DiscordSettings DiscordSettings => GetConfig().Discord;
    }
}
