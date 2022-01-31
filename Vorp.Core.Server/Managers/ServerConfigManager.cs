using Vorp.Core.Server.Models;

namespace Vorp.Core.Server.Managers
{
    public class ServerConfigManager : Manager<ServerConfigManager>
    {
        ServerConfig _serverConfig;

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
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Server Configuration was unable to be loaded.");
            }
        }

        public bool Debug => _serverConfig.Log.Debug;
        public bool Warning => _serverConfig.Log.Warn;
        public bool Error => _serverConfig.Log.Error;
        public Discord Discord => _serverConfig.Discord;
        public SqlConfig SqlConfig => _serverConfig.SqlConfig;
        public UserConfig UserConfig => _serverConfig.UserConfig;

    }
}
