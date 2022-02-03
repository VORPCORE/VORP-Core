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
        public PlayerList PlayersList => PluginManager.PlayersList;
        public ConcurrentDictionary<string, User> UserSessions => PluginManager.UserSessions;

        public EventHandlerDictionary EventRegistry => Instance.EventRegistry;
        public ExportDictionary ExportDictionary => Instance.ExportDictionary;

        protected Manager()
        {
            Instance = PluginManager.Instance;
        }

        public virtual void Begin()
        {

        }
    }
}
