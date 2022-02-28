using Vorp.Core.Client.Environment;

namespace Vorp.Core.Client
{
    static class ClientConfiguration
    {
        static ClientConfig _config;
        private static ClientConfig LoadConfiguration()
        {
            try
            {
                if (_config is not null)
                    return _config;

                string file = LoadResourceFile(GetCurrentResourceName(), $"/Resources/client-config.json");
                _config = JsonConvert.DeserializeObject<ClientConfig>(file);

                //string languagesFile = LoadResourceFile(GetCurrentResourceName(), $"/Resources/Languages/{_serverConfig.Language}.json");
                //_serverLanguage = JsonConvert.DeserializeObject<Dictionary<string, string>>(languagesFile);
                //Logger.Info($"Language '{_serverConfig.Language}.json' loaded!");

                Logger.Trace($"Client Configuration Loaded");

                return _config;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Client Configuration was unable to be loaded.");
                return (ClientConfig)default!;
            }
        }

        public static ClientConfig Config => LoadConfiguration();
    }
}
