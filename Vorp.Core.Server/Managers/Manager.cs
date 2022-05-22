using System.Collections.Concurrent;
using Vorp.Core.Server.Events;
using Vorp.Shared.Records;

namespace Vorp.Core.Server.Managers
{
    public abstract class Manager<T> where T : Manager<T>, new()
    {
        public static T GetModule()
        {
            return null;
        }

        public PluginManager Instance { get; private set; }
        public PlayerList PlayersList => PluginManager.PlayersList;
        public ConcurrentDictionary<string, User> UserSessions => PluginManager.UserSessions;

        public void Event(string name, Delegate @delegate) => Instance.Hook(name, @delegate);
        public ExportDictionary ExportDictionary => Instance.ExportDictionary;
        public ServerGateway ServerGateway => Instance.Events;
        public bool IsOneSyncEnabled => PluginManager.IsOneSyncEnabled;

        protected Manager()
        {
            Instance = PluginManager.Instance;
        }

        public virtual void Begin()
        {

        }
    }
}
