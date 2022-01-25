using CitizenFX.Core;
using System;

namespace Vorp.Core.Server
{
    public class PluginManager : BaseScript
    {
        public static PluginManager Instance { get; private set; }
        public static PlayerList PlayerList { get; private set; }

        public PluginManager()
        {
            PlayerList = Players;
            Instance = this;
        }
    }
}
