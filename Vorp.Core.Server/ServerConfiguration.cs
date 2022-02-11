using System.Collections.Generic;
using Vorp.Core.Server.Models;

namespace Vorp.Core.Server
{
    static class ServerConfiguration
    {
        static ServerConfig _serverConfig;
        static Dictionary<string, string> _serverLanguage = new();

        private static ServerConfig LoadConfiguration()
        {
            try
            {
                if (_serverConfig is not null)
                    return _serverConfig;

                _serverConfig = JsonConvert.DeserializeObject<ServerConfig>(Properties.Resources.serverConfig);
                string languagesFile = LoadResourceFile(GetCurrentResourceName(), $"/server/Resources/Languages/{_serverConfig.Language}.json");
                _serverLanguage = JsonConvert.DeserializeObject<Dictionary<string, string>>(languagesFile);
                Logger.Info($"Language '{_serverConfig.Language}.json' loaded!");

                Logger.Info($"Config: Max Characters; {_serverConfig.UserConfig.Characters.Maximum}");

                return _serverConfig;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Server Configuration was unable to be loaded.");
                return (ServerConfig)default!;
            }
        }

        public static ServerConfig Config()
        {
            return LoadConfiguration();
        }
        public static bool Debug => Config().Log.Debug;
        public static bool Warning => Config().Log.Warn;
        public static bool Error => Config().Log.Error;
        public static Discord Discord => Config().Discord;
        public static SqlConfig SqlConfig => Config().SqlConfig;
        public static UserConfig UserConfig => Config().UserConfig;

        public static int MaximumCharacters => Config().UserConfig.Characters.Maximum;

        // whitelists
        public static bool IsWhitelistDatabase => Config().WhitelistType == "database";
        public static bool IsWhitelistDiscord => Config().WhitelistType == "discord";

        public static string GetTranslation(string key)
        {
            if (_serverLanguage.ContainsKey(key))
                return $"Translation for '{key}' not found.";

            return _serverLanguage[key];
        }
    }
}
