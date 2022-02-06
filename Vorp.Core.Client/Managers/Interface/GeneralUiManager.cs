using System.Threading.Tasks;
using Vorp.Core.Client.RedM.Enums;

namespace Vorp.Core.Client.Managers.Interface
{
    public class GeneralUiManager : Manager<GeneralUiManager>
    {
        public override void Begin()
        {
            
        }

        [TickHandler(SessionWait = true)]
        private async Task OnDisableHud()
        {
            API.DisableControlAction(0, (uint)eControls.HudSpecial, true);
            API.DisableControlAction(0, (uint)eControls.RevealHud, true);
        }
    }
}
