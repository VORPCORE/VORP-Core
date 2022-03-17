using Vorp.Core.Client.Environment;

namespace Vorp.Core.Client
{
    static class ClientConfiguration
    {
        static ClientConfig _config;
        static Dictionary<string, string> _language = new();

        private static ClientConfig LoadConfiguration()
        {
            try
            {
                if (_config is not null)
                    return _config;

                string file = LoadResourceFile(GetCurrentResourceName(), $"/Resources/client-config.json");
                _config = JsonConvert.DeserializeObject<ClientConfig>(file);

                string selectedLanguage = GetResourceKvpString2("vorp:core:language");

                if (string.IsNullOrEmpty(selectedLanguage))
                    selectedLanguage = _config.Language.DefaultLanguage;

                Logger.Info($"Language '{selectedLanguage}.json' loading");

                string languagesFile = LoadResourceFile(GetCurrentResourceName(), $"/Resources/Languages/{selectedLanguage}.json");

                if (!string.IsNullOrEmpty(languagesFile))
                {
                    _language = JsonConvert.DeserializeObject<Dictionary<string, string>>(languagesFile);
                    Logger.Info($"Language '{selectedLanguage}.json' loaded!");
                }

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

        public static string Translation(string key)
        {
            if (_language.Count == 0)
            {
                LoadConfiguration();
            }

            if (!_language.ContainsKey(key))
                return $"Translation for '{key}' not found.";

            return _language[key];
        }
    }
}
