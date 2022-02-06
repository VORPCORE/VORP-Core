using System;

namespace Vorp.Core.Client.Managers.Legacy
{
    public class LegacyUiCoreManager : Manager<LegacyUiCoreManager>
    {
        public override void Begin()
        {
            Event("vorp:updateUi", new Action<string>(OnUpdateUI));
            Event("vorp:showUi", new Action<bool>(OnShowUI));
        }

        void OnUpdateUI(string stringJson)
        {
            API.SendNuiMessage(stringJson);
        }

        void OnShowUI(bool active)
        {
            string jsonpost = "{\"type\": \"ui\",\"action\":\"hide\"}";
            if (active)
            {
                jsonpost = "{\"type\": \"ui\",\"action\":\"show\"}";
            }
            API.SendNuiMessage(jsonpost);
        }

    }
}
