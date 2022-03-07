namespace Vorp.Core.Client.RedM
{
    internal class VorpAPI
    {
        public static Random Random = new Random();

        public static void SetMpGamerTagVisibility(int gamerTagId, eUIGamertagVisibility visibility)
        {
            Function.Call((Hash)0x93171DDDAB274EB8, gamerTagId, (int)visibility);
        }

        public static int CreateGamerTag(int playerId, string name, bool pointedTag = false, bool isRockstarClan = false, string clanTag = "", int clanFlag = 0)
        {
            return Function.Call<int>((Hash)0xD877AF112AD2B41B, playerId, name, pointedTag, isRockstarClan, clanTag, clanFlag);
        }

        public static float Distance(Vector3 start, Vector3 end)
        {
            return Vdist(start.X, start.Y, start.Z, end.X, end.Y, end.Z);
        }

        public static Camera CreateCameraWithParams(Vector3 position, Vector3 rotation, float fov)
        {
            int handle = CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, fov, true, 2);
            return new Camera(handle);
        }

        public static Camera RenderingCamera
        {
            get
            {
                return new Camera(API.GetRenderingCam());
            }
            set
            {
                if (value == null)
                {
                    API.RenderScriptCams(false, false, 3000, true, false, 0);
                }
                else
                {
                    value.IsActive = true;
                    API.RenderScriptCams(true, false, 3000, true, false, 0);
                }
            }
        }

        public static void DrawText(string text, Vector2 pos, float scale = 1f)
        {
            DrawText(text, x: pos.X, y: pos.Y, fontsize: scale);
        }

        public static void DrawText(string text, int font = 1, float x = 0, float y = 0, float fontscale = 1, float fontsize = 1, int r = 255, int g = 255, int b = 255, int alpha = 255, bool textcentred = false, bool shadow = false)
        {
            long str = Function.Call<long>(Hash._CREATE_VAR_STRING, 10, "LITERAL_STRING", text);
            Function.Call(Hash.SET_TEXT_SCALE, fontscale, fontsize);
            Function.Call(Hash._SET_TEXT_COLOR, r, g, b, alpha);
            Function.Call(Hash.SET_TEXT_CENTRE, textcentred);
            if (shadow) { Function.Call(Hash.SET_TEXT_DROPSHADOW, 1, 0, 0, 255); }
            Function.Call(Hash.SET_TEXT_FONT_FOR_CURRENT_COMMAND, font);
            Function.Call(Hash._DISPLAY_TEXT, str, x, y);
        }

        /// <summary>
        /// Doesn't work, currently is throwing an error
        /// </summary>
        /// <param name="title"></param>
        /// <param name="subTitle"></param>
        /// <param name="duration"></param>
        public unsafe static void DisplayLeftNotification(string title, string subTitle, int duration = 10000)
        {
            try
            {
                int* struct1 = stackalloc int[1];
                struct1[0] = duration;

                long longTitle = Function.Call<long>(Hash._CREATE_VAR_STRING, 10, "LITERAL_STRING", title);
                long longSubTitle = Function.Call<long>(Hash._CREATE_VAR_STRING, 10, "LITERAL_STRING", subTitle);

                long* struct2 = stackalloc long[4];
                struct2[0] = longTitle;
                struct2[1] = longSubTitle;
                struct2[2] = GetHashKey("HUD_TOASTS");
                struct2[3] = GetHashKey("toast_mp_status_change");

                Function.Call((Hash)0x26e87218390e6729, struct1, struct2, 1, 1);
                Logger.Trace($"DisplayLeftNotification: {title}/{subTitle}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"DisplayLeftNotification");
            }
        }

        public static async Task RequestModel(uint hash)
        {
            if (Function.Call<bool>(Hash.IS_MODEL_VALID, hash))
            {
                Function.Call(Hash.REQUEST_MODEL, hash);
                while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, hash))
                {
                    await BaseScript.Delay(100);
                }
            }
            else
            {
                Debug.WriteLine($"Model {hash} is not valid!");
            }
        }

        public static async Task<Ped> CreatePed(string model, Vector3 position, float heading)
        {
            uint modelHash = (uint)GetHashKey(model);

            await VorpAPI.RequestModel(modelHash);

            float groundZ = position.Z;
            Vector3 norm = position;
            if (API.GetGroundZAndNormalFor_3dCoord(position.X, position.Y, position.Z, ref groundZ, ref norm))
                norm = new Vector3(position.X, position.Y, groundZ);

            return new Ped(API.CreatePed(modelHash, norm.X, norm.Y, norm.Z, heading, false, true, true, true));
        }

        public static void StartSoloTutorialSession() => NetworkStartSoloTutorialSession();
        public static void EndTutorialSession() => NetworkEndTutorialSession();

        public static async Task GetImap(int hash)
        {
            while (!Function.Call<bool>(Hash._IS_IMAP_ACTIVE, hash))
            {
                Function.Call(Hash._REQUEST_IMAP, hash);
                await BaseScript.Delay(100);
            }
        }

        public static int RequestTexture(long albedo, long normal, long material) => Function.Call<int>((Hash)0xC5E7204F322E49EB, albedo, normal, material);

        public static bool IsTextureValid(int textureId) => Function.Call<bool>((Hash)0x31DC8D3F216D8509, textureId);
    }
}
