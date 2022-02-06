using System;
using System.Collections.Generic;
using System.Linq;

namespace Vorp.Core.Client.Managers.Legacy
{
    public class LegacyServerCallbackManager : Manager<LegacyServerCallbackManager>
    {
        List<CallbackDelegate> _callbackHandlers = new();
        public static int RequestId = 0;

        bool _shownWarning = false;

        public override void Begin()
        {
            EventHandler.Add("vorp:ExecuteServerCallBack", new Action<string, CallbackDelegate, object>(OnTriggerServerCallback));
            EventHandler.Add("vorp:ServerCallback", new Action<int, object>(OnServerCallback));
        }

        private void OnServerCallback(int requestId, dynamic args)
        {
            if (_callbackHandlers.ElementAt(requestId) != null)
            {
                _callbackHandlers[requestId](args);
                _callbackHandlers[requestId] = null;
            }
            else
            {
                Logger.Error("Error Server CallBack Not Found");
            }

        }

        private void OnTriggerServerCallback(string name, CallbackDelegate ncb, object args)
        {
            if (!_shownWarning)
            {
                Logger.Warn($"vorp:ExecuteServerCallBack will be deprecated, please change your own resource to no longer use this event.");
            }
            _shownWarning = true;

            _callbackHandlers.Add(ncb);

            BaseScript.TriggerServerEvent("vorp:TriggerServerCallback", name, RequestId, args);

            if (RequestId < 65565)
            {
                RequestId += 1;
            }
            else
            {
                RequestId = 0;
                _callbackHandlers.Clear();
            }
        }
    }
}
