using System;
using System.Collections.Generic;
using System.Text;

namespace Vorp.Core.Server.Managers.Legacy
{
    /// <summary>
    /// Exists only to support current resources, methods will be deprecated over time to better processes.
    /// </summary>
    public class LegacyCallbackManager : Manager<LegacyCallbackManager>
    {
        public static Dictionary<string, CallbackDelegate> Callbacks = new Dictionary<string, CallbackDelegate>();

        public override void Begin()
        {
            Logger.Info($"[MANAGER] Legacy Manager Init");
            Instance.EventRegistry.Add("vorp:addNewCallBack", new Action<string, CallbackDelegate>(OnAddNewCallback));
        }

        private void OnAddNewCallback(string name, CallbackDelegate cb)
        {
            string invokingResource = GetInvokingResource();
            if (Callbacks.ContainsKey(name))
            {
                Logger.Error($"[LegacyCallbackManager] Callback '{name}' has been updated by '{invokingResource}'.");
                return;
            }
            Logger.Info($"[LegacyCallbackManager] Registered '{invokingResource}' callback called '{name}'.");
            Callbacks.Add(name, cb);
        }
    }
}
