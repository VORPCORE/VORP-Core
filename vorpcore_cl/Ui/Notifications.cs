using CitizenFX.Core;
using System;

namespace vorpcore_cl.Ui
{
    public class Notifications : BaseScript
    {
        public Notifications()
        {
            //Notifications Based on Key_Value & Disquse & Vespura natives | Thanks for share.
            EventHandlers["vorp:Tip"] += new Action<string, int>(notifyTip);
            EventHandlers["vorp:TipRight"] += new Action<string, int>(notifyDisplayRightTip);
            EventHandlers["vorp:TipBottom"] += new Action<string, int>(notifyDisplayObjetive);
            EventHandlers["vorp:NotifyTop"] += new Action<string, string, int>(notifyDisplayTopCenterNotification);
            EventHandlers["vorp:NotifyLeft"] += new Action<string, string, string, string, int>(notifyDisplayLeftNotification);
        }

        private async void notifyDisplayLeftNotification(string title, string text, string dic, string icon, int miliseconds)
        {
            await Utils.Miscellanea.LoadTexture(dic);

            Exports["vorp_core"].DisplayLeftNotification(title, text, dic, icon, miliseconds);
        }

        private void notifyDisplayTopCenterNotification(string text, string location, int miliseconds)
        {
            Exports["vorp_core"].DisplayTopCenterNotification(text, location, miliseconds);
        }

        private void notifyDisplayObjetive(string text, int miliseconds)
        {
            Exports["vorp_core"].DisplayObjetive(text, miliseconds);
        }

        private void notifyDisplayRightTip(string text, int miliseconds)
        {
            Exports["vorp_core"].DisplayRightTip(text, miliseconds);
        }

        private void notifyTip(string text, int miliseconds)
        {
            Exports["vorp_core"].DisplayTip(text, miliseconds);
        }
    }
}
