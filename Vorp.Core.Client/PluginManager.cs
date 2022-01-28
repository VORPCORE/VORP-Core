global using CitizenFX.Core;
global using static CitizenFX.Core.Native.API;
global using Vorp.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vorp.Core.Client.Events;

namespace Vorp.Core.Client
{
    public class PluginManager : BaseScript
    {
        public static PluginManager Instance { get; private set; }
        private ClientGateway _events;

        public PluginManager()
        {
            Instance = this;
            _events = new ClientGateway(this);
        }

        public void Hook(string eventName, Delegate @delegate)
        {
            EventHandlers[eventName] += @delegate;
        }
    }
}
