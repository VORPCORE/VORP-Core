using System.Collections.Generic;
using Vorp.Core.Server.Models;

namespace Vorp.Core.Server.Managers
{
    public class ServerConfigManager : Manager<ServerConfigManager>
    {
        ServerConfig _serverConfig;

        Dictionary<string, string> _serverLanguage = new();

        public override void Begin()
        {
            LoadConfiguration();
        }

        void LoadConfiguration()
        {
            _serverConfig = new ServerConfig();
            try
            {
                _serverConfig = JsonConvert.DeserializeObject<ServerConfig>(Properties.Resources.serverConfig);
                string languagesFile = LoadResourceFile(GetCurrentResourceName(), $"Resources/Languages/{_serverConfig.Language}.json");
                _serverLanguage = JsonConvert.DeserializeObject<Dictionary<string, string>>(languagesFile);
                Logger.Info($"Language '{_serverConfig.Language}.json' loaded!");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Server Configuration was unable to be loaded.");
            }
        }

        public ServerConfig Config => _serverConfig;
        public bool Debug => _serverConfig.Log.Debug;
        public bool Warning => _serverConfig.Log.Warn;
        public bool Error => _serverConfig.Log.Error;
        public Discord Discord => _serverConfig.Discord;
        public SqlConfig SqlConfig => _serverConfig.SqlConfig;
        public UserConfig UserConfig => _serverConfig.UserConfig;

        // whitelists
        public bool IsWhitelistDatabase => _serverConfig.WhitelistType == "database";
        public bool IsWhitelistDiscord => _serverConfig.WhitelistType == "discord";

        public string GetTranslation(string key)
        {
            if (_serverLanguage.ContainsKey(key))
                return $"Translation for '{key}' not found.";

            return _serverLanguage[key];
        }

    }
}
