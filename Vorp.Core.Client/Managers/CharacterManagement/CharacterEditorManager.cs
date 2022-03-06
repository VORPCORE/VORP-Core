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
        static Ped _ped;

        public override void Begin()
        {
            Instance.Hook("onResourceStop", new Action<string>(resourceName =>
            {
                if (GetCurrentResourceName() != resourceName) return;

                Dispose();
            }));
        }

        internal static async void Init(Ped ped)
        {
            _ped = ped;

            _ped.Heading = 93.2f;
            _ped.Position = new Vector3(-558.3258f, -3781.111f, 237.60f);

            RenderScriptCams(true, true, 250, true, true, 0);
            Vector3 rot = new Vector3(-13.56231f, 0.00f, -91.93626f);
            float fov = 45f;
            _cameraMain = VorpAPI.CreateCameraWithParams(new Vector3(-560.83f, -3776.33f, 239.1f), rot, fov);
            _cameraMain.IsActive = true;

            _cameraFace = VorpAPI.CreateCameraWithParams(new Vector3(-558.9781f, -3780.955f, 239.1f), rot, fov);
            _cameraBody = VorpAPI.CreateCameraWithParams(new Vector3(-560.6195f, -3780.708f, 239.1f), rot, fov);
            _cameraWaist = VorpAPI.CreateCameraWithParams(new Vector3(-559.1779f, -3780.964f, 238.5f), rot, fov);
            _cameraLegs = VorpAPI.CreateCameraWithParams(new Vector3(-559.2103f, -3781.039f, 238.5f), rot, fov);

            _worldTime = new WorldTime(12, 1);

            await Screen.FadeIn(500);
        }

        void Dispose()
        {
            if (_worldTime is not null) _worldTime.Stop();
            if (_ped is not null) _ped.Delete();

            RenderScriptCams(false, true, 250, true, true, 0);
            _cameraMain.Delete();
            _cameraFace.Delete();
            _cameraBody.Delete();
            _cameraWaist.Delete();
            _cameraLegs.Delete();
        }
    }
}
