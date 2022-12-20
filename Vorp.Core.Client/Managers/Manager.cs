﻿using Logger;

namespace Vorp.Core.Client.Managers
{
    public abstract class Manager<T> where T : Manager<T>, new()
    {
        public static T GetModule()
        {
            return PluginManager.Instance.GetManager<T>() ?? (!PluginManager.Instance.IsLoadingManager<T>()
                       ? (T)PluginManager.Instance.LoadManager(typeof(T))
                       : null);
        }

        public PluginManager Instance { get; set; }
        public Log Logger => PluginManager.Logger;
        public void Event(string eventName, Delegate @delegate) => Instance.Hook(eventName, @delegate);

        protected Manager()
        {
            Instance = PluginManager.Instance;
        }

        public virtual void Begin()
        {
            // Ignored
        }
    }
}
