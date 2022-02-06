using System;

namespace Vorp.Core.Client.Managers.Legacy
{
    public class LegacyInstanceManager : Manager<LegacyInstanceManager>
    {
        public override void Begin()
        {
            EventHandler.Add("vorp:setInstancePlayer", new Action<bool>(OnSetPlayerInstance));
        }

        void OnSetPlayerInstance(bool instance)
        {

            switch (instance)
            {
                case true:
                    NetworkStartSoloTutorialSession();
                    break;
                case false:
                    NetworkEndTutorialSession();
                    break;
            }
        }
    }
}
