﻿using System.Collections.Generic;

namespace Vorp.Core.Server.Managers.Legacy
{
    /// <summary>
    /// Exists only to support current resources, methods will be deprecated over time to better processes.
    /// </summary>
    public class LegacyCallbackManager : Manager<LegacyCallbackManager>
    {
        public static LegacyCallbackManager CallbackManagerInstance;
        public Dictionary<string, CallbackDelegate> Callbacks = new Dictionary<string, CallbackDelegate>();

        public override void Begin()
        {
            Logger.Info($"[MANAGER] Legacy Callback Manager Init");
            Event("vorp:addNewCallBack", new Action<string, CallbackDelegate>(OnAddNewCallback));
            Event("vorp:TriggerServerCallback", new Action<Player, string, int, object>(OnTriggerServerCallback));

            CallbackManagerInstance = this;
        }

        private void OnTriggerServerCallback([FromSource] Player source, string name, int requestId, object args)
        {
            try
            {
                int _source = int.Parse(source.Handle);
                if (Callbacks.ContainsKey(name))
                {
                    Callbacks[name](_source, new Action<dynamic>((data) =>
                    {
                        source.TriggerEvent("vorp:ServerCallback", requestId, data);
                    }), args);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[LegacyCallbackManager] Callback '{name}' failed.");
                Logger.Error(ex.Message);
            }
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
