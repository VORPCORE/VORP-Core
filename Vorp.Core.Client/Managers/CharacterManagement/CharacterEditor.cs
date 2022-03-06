using Vorp.Core.Client.Interface;

namespace Vorp.Core.Client.Managers.CharacterManagement
{
    public class CharacterEditor
    {
        /*
         * Requirements
         * -> Character options for selected sex needs to update the screen
         * -> NUI Updates
         * 
         * 
         * */

        static Camera _cameraMain;
        static Camera _cameraFace;
        static Camera _cameraWaist;
        static Camera _cameraLegs;
        static Camera _cameraBody;

        internal static async void Init()
        {
            RenderScriptCams(true, true, 250, true, true, 0);
            Vector3 rot = new Vector3(-13.56231f, 0.00f, -91.93626f);
            float fov = 45f;
            _cameraMain = VorpAPI.CreateCameraWithParams(new Vector3(-560.83f, -3776.33f, 239.58f), rot, fov);
            _cameraMain.IsActive = true;
            _cameraFace = VorpAPI.CreateCameraWithParams(new Vector3(-558.9781f, -3780.955f, 239.186f), rot, fov);
            _cameraBody = VorpAPI.CreateCameraWithParams(new Vector3(-560.6195f, -3780.708f, 239.1954f), rot, fov);
            _cameraWaist = VorpAPI.CreateCameraWithParams(new Vector3(-559.1779f, -3780.964f, 238.4654f), rot, fov);
            _cameraLegs = VorpAPI.CreateCameraWithParams(new Vector3(-559.2103f, -3781.039f, 238.4678f), rot, fov);

            await Screen.FadeIn(500);
        }

        void Dispose()
        {
            RenderScriptCams(false, true, 250, true, true, 0);
            _cameraMain.Delete();
            _cameraFace.Delete();
            _cameraBody.Delete();
            _cameraWaist.Delete();
            _cameraLegs.Delete();
        }
    }
}
