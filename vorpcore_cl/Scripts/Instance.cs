using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;

namespace vorpcore_cl.Scripts
{
    public class Instance : BaseScript
    {
        private static bool intancePlayer = false;
        public Instance()
        {
            EventHandlers["vorp:setInstancePlayer"] += new Action<bool>(SetInstancePlayer);
        }

        private void SetInstancePlayer(bool instance)
        {

            switch (instance)
            {
                case true:
                    Function.Call((Hash)0x17E0198B3882C2CB);
                    break;
                case false:
                    Function.Call((Hash)0xD0AFAFF5A51D72F7);
                    break;
            }
        }
    }
}
