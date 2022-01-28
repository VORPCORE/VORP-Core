global using CitizenFX.Core;
global using static CitizenFX.Core.Native.API;
global using Vorp.Diagnostics;
using System;
using Vorp.Core.Server.Events;

namespace Vorp.Core.Server
{
    public class PluginManager : BaseScript
    {
        public static PluginManager Instance { get; private set; }
        public static PlayerList PlayerList { get; private set; }

        private ServerGateway _events;

        public PluginManager()
        {
            PlayerList = Players;
            _events = new ServerGateway(this);
            Instance = this;
        }

        public void Hook(string eventName, Delegate @delegate)
        {
            EventHandlers[eventName] += @delegate;
        }

        public static Player ToPlayer(int handle)
        {
            return PluginManager.Instance.Players[handle];
        }
    }
}