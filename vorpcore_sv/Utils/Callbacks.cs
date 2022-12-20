using CitizenFX.Core;
using System;
using System.Collections.Generic;

namespace vorpcore_sv.Utils
{
    class Callbacks : BaseScript
    {
        public static Dictionary<string, CallbackDelegate> ServerCallBacks = new Dictionary<string, CallbackDelegate>();

        public Callbacks()
        {
            EventHandlers["vorp:addNewCallBack"] += new Action<string, CallbackDelegate>(addNewCallBack);
            EventHandlers["vorp:TriggerServerCallback"] += new Action<Player, string, int, object>(triggerServerCallback);
        }

        public async void triggerServerCallback([FromSource] Player source, string name, int requestId, object args)
        {
            try
            {
                int _source = int.Parse(source.Handle);
                await Delay(100);
                if (ServerCallBacks.ContainsKey(name))
                {
                    ServerCallBacks[name](_source, new Action<dynamic>(async (data) =>
                    {
                        source.TriggerEvent("vorp:ServerCallback", requestId, data);
                    }), args);
                }
            }
            catch
            {
                Debug.WriteLine($"Failed Callback {name}");
            }
        }

        private void addNewCallBack(string name, CallbackDelegate ncb)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Vorp Core: {name} function callback registered!");
            Console.ForegroundColor = ConsoleColor.White;

            ServerCallBacks[name] = ncb;
        }
    }
}
