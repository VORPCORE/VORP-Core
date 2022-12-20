using CitizenFX.Core;
using System;
using System.Collections.Generic;
using vorpcore_cl.Utils;

namespace vorpcore_cl
{
    public class vorpcore_cl : BaseScript
    {
        public vorpcore_cl()
        {
            EventHandlers["getCore"] += new Action<CallbackDelegate>((cb) =>
            {
                Dictionary<string, dynamic> corefunctions = new Dictionary<string, dynamic>
                {
                    ["RpcCall"] = new Action<string, CallbackDelegate, object>((name, callback, args) =>
                      {
                          Callback.ServerCallBacks.Add(callback);

                          TriggerServerEvent("vorp:TriggerServerCallback", name, Callback.RequestId, args);

                          if (Callback.RequestId < 65565)
                          {
                              Callback.RequestId += 1;
                          }
                          else
                          {
                              Callback.RequestId = 0;
                              Callback.ServerCallBacks.Clear();
                          }
                      }),
                    ["displayTip"] = new Action<string, int>((text, miliseconds) =>
                     {
                         Exports["vorp_core"].DisplayTip(text, miliseconds);
                     }),
                    ["displayRightTip"] = new Action<string, int>((text, miliseconds) =>
                     {
                         Exports["vorp_core"].DisplayRightTip(text, miliseconds);
                     }),
                    ["displayObjetive"] = new Action<string, int>((text, miliseconds) =>
                     {
                         Exports["vorp_core"].DisplayObjetive(text, miliseconds);
                     }),
                    ["displayTopCenterNotification"] = new Action<string, string, int>((text, location, miliseconds) =>
                      {
                          Exports["vorp_core"].DisplayTopCenterNotification(text, location, miliseconds);
                      }),
                    ["displayLeftNotification"] = new Action<string, string, string, string, int>(async (title, text, dic, icon, miliseconds) =>
                        {
                            await Miscellanea.LoadTexture(dic);
                            Exports["vorp_core"].DisplayLeftNotification(title, text, dic, icon, miliseconds);
                        }),
                };
                cb.Invoke(corefunctions);
            });
        }

    }
}
