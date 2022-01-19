using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace vorpcore_cl.Utils
{
    class Callback : BaseScript
    {
        public static List<CallbackDelegate> ServerCallBacks = new List<CallbackDelegate>();
        public static int RequestId = 0;

        public Callback()
        {
            EventHandlers["vorp:ExecuteServerCallBack"] += new Action<string, CallbackDelegate, object>(triggerServerCallBack);
            EventHandlers["vorp:ServerCallback"] += new Action<int, object>(serverCallback);
        }

        private void serverCallback(int requestId, dynamic args)
        {
            if (ServerCallBacks.ElementAt(requestId) != null)
            {
                ServerCallBacks[requestId](args);
                ServerCallBacks[requestId] = null;
            }
            else
            {
                Debug.WriteLine("VorpCore: Error Server CallBack Not Found");
            }

        }

        private void triggerServerCallBack(string name, CallbackDelegate ncb, object args)
        {
            ServerCallBacks.Add(ncb);

            TriggerServerEvent("vorp:TriggerServerCallback", name, RequestId, args);

            if (RequestId < 65565)
            {
                RequestId += 1;
            }
            else
            {
                RequestId = 0;
                ServerCallBacks.Clear();
            }
        }
    }
}
