using Vorp.Core.Client.Interface;
using Vorp.Shared.Models;

namespace Vorp.Core.Client.Managers.CharacterManagement
{
    public class CharacterEditorManager : Manager<CharacterEditorManager>
    {
        /*
         * Requirements
         * -> Character options for selected sex needs to update the screen
         * -> NUI Updates
         * 
         * 
         * */

        private static PluginManager Instance => PluginManager.Instance;

        static Vector3 _pedPosition = new Vector3(-558.3258f, -3781.111f, 237.60f);
        static float _pedHeading = 93.2f;
        static Ped _ped;

        static Camera _cameraMain;
        static Camera _cameraFace;
        static Camera _cameraWaist;
        static Camera _cameraLegs;
        static Camera _cameraBody;

        static WorldTime _worldTime;

        static Prompt _promptRandomise;

        static bool _hideNui;

        public override void Begin()
        {
            Instance.Hook("onResourceStop", new Action<string>(resourceName =>
            {
                if (GetCurrentResourceName() != resourceName) return;

                Dispose();
            }));
        }

        internal static async void Init(string pedModel, VorpPedComponents components)
        {
            _ped = await VorpAPI.CreatePed(pedModel, _pedPosition, _pedHeading);
            _ped.IsPositionFrozen = true;
            SetEntityInvincible(_ped.Handle, true);
            _ped.PedComponents = components;

            RenderScriptCams(true, true, 250, true, true, 0);
            Vector3 rot = new Vector3(-1.06042f, 0.00f, -90.58475f);
            float fov = 37f;
            _cameraMain = VorpAPI.CreateCameraWithParams(new Vector3(-561.223f, -3780.933f, 238.9249f), rot, fov);
            _cameraMain.IsActive = true;

            _cameraFace = VorpAPI.CreateCameraWithParams(new Vector3(-561.223f, -3780.933f, 238.9249f), rot, fov);
            _cameraBody = VorpAPI.CreateCameraWithParams(new Vector3(-561.223f, -3780.933f, 238.9249f), rot, fov);
            _cameraWaist = VorpAPI.CreateCameraWithParams(new Vector3(-561.223f, -3780.933f, 238.9249f), rot, fov);
            _cameraLegs = VorpAPI.CreateCameraWithParams(new Vector3(-561.223f, -3780.933f, 238.9249f), rot, fov);

            _worldTime = new WorldTime(12, 1);

            _promptRandomise = Prompt.Create(eControl.ContextX, "Randomise");
            Instance.AttachTickHandler(OnPromptHandler);

            DisplayHud(false);
            DisplayRadar(false);

            Instance.NuiManager.Toggle("character/VISIBLE");

            // SetNuiFocus2(true, true);

            await Screen.FadeIn(500);
        }

        private static async Task OnPromptHandler()
        {
            if (_promptRandomise.IsPressed)
            {
                _ped.RandomiseClothingAsync(true);
                await BaseScript.Delay(500);
            }

            if (IsPauseMenuActive() && !_hideNui)
            {
                _hideNui = true;
                Instance.NuiManager.Toggle("character/VISIBLE");
            }
            else if (!IsPauseMenuActive() && _hideNui)
            {
                _hideNui = false;
                Instance.NuiManager.Toggle("character/VISIBLE");
            }
        }

        void Dispose()
        {
            if (_worldTime is not null) _worldTime.Stop();

            DisplayHud(false);
            DisplayRadar(false);

            _ped.Delete();

            SetNuiFocus2(false, false);

            _promptRandomise.Delete();

            RenderScriptCams(false, true, 250, true, true, 0);
            _cameraMain.Delete();
            _cameraFace.Delete();
            _cameraBody.Delete();
            _cameraWaist.Delete();
            _cameraLegs.Delete();
        }
    }
}
