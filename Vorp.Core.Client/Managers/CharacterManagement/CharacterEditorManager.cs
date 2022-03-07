using Vorp.Core.Client.Interface;
using Vorp.Core.Client.Interface.Menu;
using Vorp.Shared.Models;
using Vorp.Shared.Models.NuiResponse;

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

        static bool _hideNui;

        public override void Begin()
        {
            Instance.Hook("onResourceStop", new Action<string>(resourceName =>
            {
                if (GetCurrentResourceName() != resourceName) return;

                Dispose();
            }));

            Instance.NuiManager.RegisterCallback("CharacterRandomise", new Action(() =>
            {
                _ped.RandomiseClothingAsync(true);
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

            Instance.AttachTickHandler(OnNuiHandling);

            DisplayHud(false);
            DisplayRadar(false);

            World.SetWeather(Shared.Enums.eWeatherType.SUNNY);
            World.SetWeatherFrozen(true);
            World.WindSpeed = 0f;

            await BaseScript.Delay(1000);
            await Screen.FadeIn(500);
            
            CreateBaseMenu();
            Instance.NuiManager.SetFocus(true, true);
        }

        private static void CreateBaseMenu()
        {
            MenuBase menuBase = new();
            menuBase.Title = "Main Menu";

            MenuOptions moCharacterRandomise = new MenuOptions();
            moCharacterRandomise.Type = "button";
            moCharacterRandomise.Label = "Randomise Character";
            moCharacterRandomise.Endpoint = "CharacterRandomise";
            moCharacterRandomise.Description = "Randomises your character's appearance.";
            menuBase.AddOption(moCharacterRandomise);

            MenuOptions moCharacterName = new MenuOptions();
            moCharacterName.Type = "button";
            moCharacterName.Label = "Name";
            moCharacterName.RightLabel = "Enter Name";
            moCharacterName.Endpoint = "CharacterChangeName";
            moCharacterName.Description = "Set your character's name.";
            menuBase.AddOption(moCharacterName);

            MenuOptions moCharacterAppearance = new MenuOptions();
            moCharacterAppearance.Type = "menu";
            moCharacterAppearance.Label = "Appearance";
            moCharacterAppearance.SubTitle = "Options";
            moCharacterAppearance.Description = "Change your character's appearance.";
            menuBase.AddOption(moCharacterAppearance);

            List<long> hairs = _ped.Hairs;
            MenuOptions moCharacterHair = new();
            moCharacterHair.Type = "list";
            moCharacterHair.Label = "Hair";
            moCharacterHair.Endpoint = "CharacterSetHair";
            moCharacterHair.Description = "Change your character's hair.";
            moCharacterHair.ListMin = 1;
            moCharacterHair.ListMax = hairs.Count;
            moCharacterHair.Value = hairs.IndexOf(_ped.PedComponents.Hair.Value);
            moCharacterAppearance.AddOption(moCharacterHair);

            Instance.NuiManager.RegisterCallback("CharacterSetHair", new Action<List<string>>(args =>
            {
                Dictionary<string, int> valuePairs = JsonConvert.DeserializeObject<Dictionary<string, int>>(args[0]);
                if (!valuePairs.ContainsKey("selectedValue")) return;
                int selectIndex = valuePairs["selectedValue"] - 1; // minus 1 because of human reading
                _ped.PedComponents.Hair.Value = _ped.Hairs[selectIndex];
                _ped.UpdateComponents();
            }));

            MenuOptions moCharacterSave = new MenuOptions();
            moCharacterSave.Type = "button";
            moCharacterSave.Label = "Confirm";
            moCharacterSave.Endpoint = "CharacterConfirm";
            moCharacterSave.Description = "Confirm and save your character.";
            menuBase.AddOption(moCharacterSave);

            Instance.NuiManager.Set("character/CREATOR", menuBase);
            Instance.NuiManager.Toggle("character/VISIBLE");
        }

        private static async Task OnNuiHandling()
        {
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

            World.SetWeather(Shared.Enums.eWeatherType.SUNNY);
        }

        void Dispose()
        {
            if (_worldTime is not null) _worldTime.Stop();

            DisplayHud(false);
            DisplayRadar(false);

            if (_ped is not null) _ped.Delete();

            SetNuiFocus2(false, false);

            Instance.NuiManager.Toggle("character/VISIBLE");

            RenderScriptCams(false, true, 250, true, true, 0);
            _cameraMain.Delete();
            _cameraFace.Delete();
            _cameraBody.Delete();
            _cameraWaist.Delete();
            _cameraLegs.Delete();

            World.SetWeatherFrozen(false);
        }
    }
}
