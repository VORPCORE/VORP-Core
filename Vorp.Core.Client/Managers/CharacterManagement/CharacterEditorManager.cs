using Vorp.Core.Client.Interface;

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

        static Camera _cameraMain;
        static Camera _cameraFace;
        static Camera _cameraWaist;
        static Camera _cameraLegs;
        static Camera _cameraBody;

        static WorldTime _worldTime;

        public override void Begin()
        {
            Instance.Hook("onResourceStop", new Action<string>(resourceName =>
            {
                if (GetCurrentResourceName() != resourceName) return;

                Dispose();
            }));
        }

        internal static async void Init()
        {
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

            DisplayHud(false);
            DisplayRadar(false);

            await Screen.FadeIn(500);
        }

        void Dispose()
        {
            if (_worldTime is not null) _worldTime.Stop();

            DisplayHud(false);
            DisplayRadar(false);

            RenderScriptCams(false, true, 250, true, true, 0);
            _cameraMain.Delete();
            _cameraFace.Delete();
            _cameraBody.Delete();
            _cameraWaist.Delete();
            _cameraLegs.Delete();
        }
    }
}
