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

            Instance.NuiManager.RegisterCallback("CharacterRandomise", new Action(async () =>
            {
                _ped.RandomiseClothingAsync(true);
                await BaseScript.Delay(100);
                // Need to review updating values on the NUI Menu
                CreateBaseMenu(true);
            }));

            Instance.NuiManager.RegisterCallback("CharacterSetComponent", new Action<List<string>>(args =>
            {
                Dictionary<string, dynamic> valuePairs = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(args[0]);
                if (!valuePairs.ContainsKey("selectedValue")) return;

                int.TryParse($"{valuePairs["selectedValue"]}", out int selectIndex);
                selectIndex = selectIndex - 1; // Cause Overflow

                string component = valuePairs["component"];

                VorpPedComponents vorpPedComponents = _ped.PedComponents;

                switch(component)
                {
                    case "Head":
                        if (selectIndex <= 0)
                            vorpPedComponents.Head.Value = 0;
                        else
                            vorpPedComponents.Head.Value = _ped.Heads[selectIndex];
                        break;
                    case "Eyes":
                        if (selectIndex <= 0)
                            vorpPedComponents.Eyes.Value = 0;
                        else
                            vorpPedComponents.Eyes.Value = _ped.Eyes[selectIndex];
                        break;
                    case "Hair":
                        if (selectIndex <= 0)
                            vorpPedComponents.Hair.Value = 0;
                        else
                            vorpPedComponents.Hair.Value = _ped.Hairs[selectIndex];
                        break;
                    case "Beard":
                        if (selectIndex <= 0)
                            vorpPedComponents.Beard.Value = 0;
                        else
                            vorpPedComponents.Beard.Value = _ped.Beards[selectIndex];
                        break;
                    case "BodyUpper":
                        if (selectIndex <= 0)
                            vorpPedComponents.BodyUpper.Value = 0;
                        else
                            vorpPedComponents.BodyUpper.Value = _ped.BodiesUpper[selectIndex];
                        break;
                    case "BodyLower":
                        if (selectIndex <= 0)
                            vorpPedComponents.BodyLower.Value = 0;
                        else
                            vorpPedComponents.BodyLower.Value = _ped.BodiesLower[selectIndex];
                        break;
                }

                _ped.PedComponents = vorpPedComponents;
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

        private static void CreateBaseMenu(bool update = false)
        {
            MenuBase menuBase = new();
            menuBase.Title = "Main Menu";

            MenuOptions moCharacterRandomise = MenuOptions.MenuOptionButton("Randomise Character", "Randomises your character's appearance.", "CharacterRandomise");
            menuBase.AddOption(moCharacterRandomise);

            MenuOptions moCharacterName = MenuOptions.MenuOptionButton("Name", "Set your character's name.", "CharacterChangeName", "Enter Name");
            menuBase.AddOption(moCharacterName);

            MenuOptions moCharacterAppearance = MenuOptions.MenuOptionMenu("Appearance", "Options", "Change your character's appearance.");
            menuBase.AddOption(moCharacterAppearance);

            MenuOptions moCharacterEyes = MenuOptions.MenuOptionList("Eyes", "Change your character's eyes.", "CharacterSetComponent/Eyes", _ped.Eyes, _ped.PedComponents.Eyes.Value);
            moCharacterAppearance.AddOption(moCharacterEyes);

            MenuOptions moCharacterHead = MenuOptions.MenuOptionList("Head", "Change your character's head.", "CharacterSetComponent/Head", _ped.Heads, _ped.PedComponents.Head.Value);
            moCharacterAppearance.AddOption(moCharacterHead);

            MenuOptions moCharacterBodyUpper = MenuOptions.MenuOptionList("Upper Body", "Change your character's upper body.", "CharacterSetComponent/BodyUpper", _ped.BodiesUpper, _ped.PedComponents.BodyUpper.Value);
            moCharacterAppearance.AddOption(moCharacterBodyUpper);

            MenuOptions moCharacterBodyLower = MenuOptions.MenuOptionList("Lower Body", "Change your character's lower body.", "CharacterSetComponent/BodyLower", _ped.BodiesLower, _ped.PedComponents.BodyLower.Value);
            moCharacterAppearance.AddOption(moCharacterBodyLower);

            MenuOptions moCharacterHair = MenuOptions.MenuOptionList("Hair", "Change your character's hair.", "CharacterSetComponent/Hair", _ped.Hairs, _ped.PedComponents.Hair.Value);
            moCharacterAppearance.AddOption(moCharacterHair);

            if (_ped.IsMale)
            {
                MenuOptions moCharacterBeard = MenuOptions.MenuOptionList("Beard", "Change your character's beard.", "CharacterSetComponent/Beard", _ped.Beards, _ped.PedComponents.Beard.Value);
                moCharacterAppearance.AddOption(moCharacterBeard);
            }

            MenuOptions moCharacterSave = MenuOptions.MenuOptionButton("Confirm", "Confirm and save your character.", "CharacterConfirm");
            menuBase.AddOption(moCharacterSave);

            Instance.NuiManager.Set("character/CREATOR", menuBase);

            if (!update)
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
