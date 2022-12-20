using Logger;
using System.Collections.Concurrent;
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
        public Log Logger => PluginManager.Logger;
        public PlayerList PlayersList => PluginManager.PlayersList;
        public ConcurrentDictionary<int, User> UserSessions => PluginManager.UserSessions;

        public void Event(string name, Delegate @delegate) => Instance.Hook(name, @delegate);
        public ExportDictionary ExportDictionary => Instance.ExportDictionary;
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
