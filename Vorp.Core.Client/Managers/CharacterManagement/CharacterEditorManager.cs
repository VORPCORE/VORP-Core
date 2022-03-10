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
                try
                {
                    Dictionary<string, dynamic> valuePairs = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(args[0]);
                    if (!valuePairs.ContainsKey("selectedValue")) return;

                    int.TryParse($"{valuePairs["selectedValue"]}", out int selectIndex);
                    string component = valuePairs["component"];

                    VorpPedComponents vorpPedComponents = _ped.PedComponents;

                    switch (component)
                    {
                        case "Eyes":
                            UpdateComponent(selectIndex, _ped.Eyes, vorpPedComponents.Eyes);
                            break;
                        case "Head":
                            UpdateComponent(selectIndex, _ped.Heads, vorpPedComponents.Head);
                            break;
                        case "BodyUpper":
                            UpdateComponent(selectIndex, _ped.BodiesUpper, vorpPedComponents.BodyUpper);
                            break;
                        case "BodyLower":
                            UpdateComponent(selectIndex, _ped.BodiesLower, vorpPedComponents.BodyLower);
                            break;
                        case "Accessory":
                            UpdateComponent(selectIndex, _ped.Accessories, vorpPedComponents.Accessory);
                            break;
                        case "Armor":
                            UpdateComponent(selectIndex, _ped.Armor, vorpPedComponents.Armor);
                            break;
                        case "Badge":
                            UpdateComponent(selectIndex, _ped.Badges, vorpPedComponents.Badge);
                            break;
                        case "Beard":
                            UpdateComponent(selectIndex, _ped.Beards, vorpPedComponents.Beard);
                            break;
                        case "Belt":
                            UpdateComponent(selectIndex, _ped.Belts, vorpPedComponents.Belt);
                            break;
                        case "BeltBuckle":
                            UpdateComponent(selectIndex, _ped.BeltBuckles, vorpPedComponents.BeltBuckle);
                            break;
                        case "BootAccessory":
                            UpdateComponent(selectIndex, _ped.BootAccessories, vorpPedComponents.BootAccessory);
                            break;
                        case "Boots":
                            UpdateComponent(selectIndex, _ped.Boots, vorpPedComponents.Boots);
                            break;
                        case "Chaps":
                            UpdateComponent(selectIndex, _ped.Chaps, vorpPedComponents.Chaps);
                            break;
                        case "Cloaks":
                            UpdateComponent(selectIndex, _ped.Cloaks, vorpPedComponents.Cloak);
                            break;
                        case "Coats":
                            UpdateComponent(selectIndex, _ped.Coats, vorpPedComponents.Coat);
                            break;
                        case "CoatsClosed":
                            UpdateComponent(selectIndex, _ped.CoatsClosed, vorpPedComponents.CoatClosed);
                            break;
                        case "Dresses":
                            UpdateComponent(selectIndex, _ped.Dresses, vorpPedComponents.Dresses);
                            break;
                        case "EyeWear":
                            UpdateComponent(selectIndex, _ped.EyeWear, vorpPedComponents.EyeWear);
                            break;
                        case "Gauntlets":
                            UpdateComponent(selectIndex, _ped.Gauntlets, vorpPedComponents.Gauntlet);
                            break;
                        case "Gloves":
                            UpdateComponent(selectIndex, _ped.Gloves, vorpPedComponents.Gloves);
                            break;
                        case "Gunbelts":
                            UpdateComponent(selectIndex, _ped.Gunbelts, vorpPedComponents.Gunbelt);
                            break;
                        case "GunbeltAccessories":
                            UpdateComponent(selectIndex, _ped.GunbeltAccessories, vorpPedComponents.GunbeltAccessory);
                            break;
                        case "Hair":
                            UpdateComponent(selectIndex, _ped.Hairs, vorpPedComponents.Hair);
                            break;
                        case "HairAccessories":
                            UpdateComponent(selectIndex, _ped.HairAccessories, vorpPedComponents.HairAccessory);
                            break;
                        case "Hats":
                            UpdateComponent(selectIndex, _ped.Hats, vorpPedComponents.Hat);
                            break;
                        case "HolstersLeft":
                            UpdateComponent(selectIndex, _ped.HolstersLeft, vorpPedComponents.HolstersLeft);
                            break;
                        case "JewelryBracelets":
                            UpdateComponent(selectIndex, _ped.JewelryBracelets, vorpPedComponents.JewelryBracelets);
                            break;
                        case "JewelryRingsLeft":
                            UpdateComponent(selectIndex, _ped.JewelryRingsLeft, vorpPedComponents.JewelryRingsLeft);
                            break;
                        case "JewelryRingsRight":
                            UpdateComponent(selectIndex, _ped.JewelryRingsRight, vorpPedComponents.JewelryRingsRight);
                            break;
                        case "Loadouts":
                            UpdateComponent(selectIndex, _ped.Loadouts, vorpPedComponents.Loadout);
                            break;
                        case "Masks":
                            UpdateComponent(selectIndex, _ped.Masks, vorpPedComponents.Masks);
                            break;
                        case "Neckties":
                            UpdateComponent(selectIndex, _ped.Neckties, vorpPedComponents.Neckties);
                            break;
                        case "Neckwear":
                            UpdateComponent(selectIndex, _ped.Neckwear, vorpPedComponents.Neckwear);
                            break;
                        case "Pants":
                            UpdateComponent(selectIndex, _ped.Pants, vorpPedComponents.Pants);
                            vorpPedComponents.Skirt.Value = 0;
                            break;
                        case "Ponchos":
                            UpdateComponent(selectIndex, _ped.Ponchos, vorpPedComponents.Poncho);
                            break;
                        case "Satchels":
                            UpdateComponent(selectIndex, _ped.Satchels, vorpPedComponents.Satchel);
                            break;
                        case "Shirt":
                            UpdateComponent(selectIndex, _ped.Shirts, vorpPedComponents.Shirt);
                            break;
                        case "Skirts":
                            UpdateComponent(selectIndex, _ped.Skirts, vorpPedComponents.Skirt);
                            vorpPedComponents.Pants.Value = 0;
                            break;
                        case "Spats":
                            UpdateComponent(selectIndex, _ped.Spats, vorpPedComponents.Spats);
                            break;
                        case "Suspenders":
                            UpdateComponent(selectIndex, _ped.Suspenders, vorpPedComponents.Suspenders);
                            break;
                        case "Teeth":
                            UpdateComponent(selectIndex, _ped.Teeth, vorpPedComponents.Teeth);
                            break;
                        case "Vests":
                            UpdateComponent(selectIndex, _ped.Vests, vorpPedComponents.Vest);
                            break;
                        default:
                            Logger.Error($"Component '{component}' not configured");
                            break;
                    }

                    _ped.PedComponents = vorpPedComponents;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Error occurred when trying to change item");
                }
            }));
        }

        internal static void UpdateComponent(int index, List<long> items, PedComponent pedComponent)
        {
            index--; // minus as UI sends from list starting at 1
            
            if (index > items.Count) index = items.Count - 1;

            pedComponent.Value = index <= 0 ? 0 : items[index];
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

            Dictionary<string, Tuple<string, string, List<long>, long>> appearanceOptions = new();
            appearanceOptions.Add("CharacterSetComponent/Eyes", new Tuple<string, string, List<long>, long>("Eyes", "Change your character's eyes.", _ped.Eyes, _ped.PedComponents.Eyes.Value));
            appearanceOptions.Add("CharacterSetComponent/Head", new Tuple<string, string, List<long>, long>("Head", "Change your character's head.", _ped.Heads, _ped.PedComponents.Head.Value));
            appearanceOptions.Add("CharacterSetComponent/BodyUpper", new Tuple<string, string, List<long>, long>("Upper Body", "Change your character's upper body.", _ped.BodiesUpper, _ped.PedComponents.BodyUpper.Value));
            appearanceOptions.Add("CharacterSetComponent/BodyLower", new Tuple<string, string, List<long>, long>("Lower Body", "Change your character's lower body.", _ped.BodiesLower, _ped.PedComponents.BodyLower.Value));
            appearanceOptions.Add("CharacterSetComponent/Accessory", new Tuple<string, string, List<long>, long>("Accessory", "Change your character's accessories.", _ped.Accessories, _ped.PedComponents.Accessory.Value));
            appearanceOptions.Add("CharacterSetComponent/Armor", new Tuple<string, string, List<long>, long>("Armor", "Change your character's armor.", _ped.Armor, _ped.PedComponents.Armor.Value));
            appearanceOptions.Add("CharacterSetComponent/Badge", new Tuple<string, string, List<long>, long>("Badge", "Change your character's badge.", _ped.Badges, _ped.PedComponents.Badge.Value));
            
            if (_ped.IsMale)
                appearanceOptions.Add("CharacterSetComponent/Beard", new Tuple<string, string, List<long>, long>("Beard", "Change your character's beard.", _ped.Beards, _ped.PedComponents.Beard.Value));

            appearanceOptions.Add("CharacterSetComponent/Belt", new Tuple<string, string, List<long>, long>("Belt", "Change your character's belt.", _ped.Belts, _ped.PedComponents.Belt.Value));
            appearanceOptions.Add("CharacterSetComponent/BeltBuckle", new Tuple<string, string, List<long>, long>("Belt Buckle", "Change your character's belt buckle.", _ped.BeltBuckles, _ped.PedComponents.BeltBuckle.Value));
            appearanceOptions.Add("CharacterSetComponent/BootAccessory", new Tuple<string, string, List<long>, long>("Boot Accessory", "Change your character's boot accessory.", _ped.BootAccessories, _ped.PedComponents.BootAccessory.Value));
            appearanceOptions.Add("CharacterSetComponent/Boots", new Tuple<string, string, List<long>, long>("Boots", "Change your character's boots.", _ped.Boots, _ped.PedComponents.Boots.Value));
            appearanceOptions.Add("CharacterSetComponent/Chaps", new Tuple<string, string, List<long>, long>("Chaps", "Change your character's chaps.", _ped.Chaps, _ped.PedComponents.Chaps.Value));
            appearanceOptions.Add("CharacterSetComponent/Cloaks", new Tuple<string, string, List<long>, long>("Cloaks", "Change your character's cloak.", _ped.Cloaks, _ped.PedComponents.Cloak.Value));
            appearanceOptions.Add("CharacterSetComponent/Coats", new Tuple<string, string, List<long>, long>("Coats", "Change your character's coat.", _ped.Coats, _ped.PedComponents.Coat.Value));
            appearanceOptions.Add("CharacterSetComponent/CoatsClosed", new Tuple<string, string, List<long>, long>("Coats Closed", "Change your character's closed coat.", _ped.CoatsClosed, _ped.PedComponents.CoatClosed.Value));

            if (_ped.IsMale)
                appearanceOptions.Add("CharacterSetComponent/Dresses", new Tuple<string, string, List<long>, long>("Dresses", "Change your character's dresses.", _ped.Dresses, _ped.PedComponents.Dresses.Value));

            appearanceOptions.Add("CharacterSetComponent/EyeWear", new Tuple<string, string, List<long>, long>("Eye Wear", "Change your character's eye wear.", _ped.EyeWear, _ped.PedComponents.EyeWear.Value));
            appearanceOptions.Add("CharacterSetComponent/Gauntlets", new Tuple<string, string, List<long>, long>("Gauntlets", "Change your character's gauntlets.", _ped.Gauntlets, _ped.PedComponents.Gauntlet.Value));
            appearanceOptions.Add("CharacterSetComponent/Gloves", new Tuple<string, string, List<long>, long>("Gloves", "Change your character's gloves.", _ped.Gloves, _ped.PedComponents.Gloves.Value));
            appearanceOptions.Add("CharacterSetComponent/Gunbelts", new Tuple<string, string, List<long>, long>("Gunbelts", "Change your character's gunbelts.", _ped.Gunbelts, _ped.PedComponents.Gunbelt.Value));
            appearanceOptions.Add("CharacterSetComponent/GunbeltAccessories", new Tuple<string, string, List<long>, long>("Gunbelt Accessories", "Change your character's gunbelt accessories.", _ped.GunbeltAccessories, _ped.PedComponents.GunbeltAccessory.Value));
            appearanceOptions.Add("CharacterSetComponent/Hair", new Tuple<string, string, List<long>, long>("Hair", "Change your character's hair.", _ped.Hairs, _ped.PedComponents.Hair.Value));

            if (!_ped.IsMale)
                appearanceOptions.Add("CharacterSetComponent/HairAccessories", new Tuple<string, string, List<long>, long>("Hair Accessories", "Change your character's hair accessories.", _ped.HairAccessories, _ped.PedComponents.HairAccessory.Value));

            appearanceOptions.Add("CharacterSetComponent/Hats", new Tuple<string, string, List<long>, long>("Hats", "Change your character's hats.", _ped.Hats, _ped.PedComponents.Hat.Value));
            appearanceOptions.Add("CharacterSetComponent/HolstersLeft", new Tuple<string, string, List<long>, long>("Holsters", "Change your character's holster.", _ped.Hats, _ped.PedComponents.HolstersLeft.Value));
            appearanceOptions.Add("CharacterSetComponent/JewelryBracelets", new Tuple<string, string, List<long>, long>("Bracelets", "Change your character's bracelets.", _ped.JewelryBracelets, _ped.PedComponents.JewelryBracelets.Value));
            appearanceOptions.Add("CharacterSetComponent/JewelryRingsLeft", new Tuple<string, string, List<long>, long>("Rings Left Hand", "Change your character's rings.", _ped.JewelryRingsLeft, _ped.PedComponents.JewelryRingsLeft.Value));
            appearanceOptions.Add("CharacterSetComponent/JewelryRingsRight", new Tuple<string, string, List<long>, long>("Rings Right Hand", "Change your character's rings.", _ped.JewelryRingsRight, _ped.PedComponents.JewelryRingsRight.Value));
            appearanceOptions.Add("CharacterSetComponent/Loadouts", new Tuple<string, string, List<long>, long>("Loadouts", "Change your character's loadouts.", _ped.Loadouts, _ped.PedComponents.Loadout.Value));
            appearanceOptions.Add("CharacterSetComponent/Masks", new Tuple<string, string, List<long>, long>("Masks", "Change your character's masks.", _ped.Masks, _ped.PedComponents.Masks.Value));
            appearanceOptions.Add("CharacterSetComponent/Neckties", new Tuple<string, string, List<long>, long>("Neckties", "Change your character's necktie.", _ped.Neckties, _ped.PedComponents.Neckties.Value));
            appearanceOptions.Add("CharacterSetComponent/Neckwear", new Tuple<string, string, List<long>, long>("Neckwear", "Change your character's neckwear.", _ped.Neckwear, _ped.PedComponents.Neckwear.Value));
            appearanceOptions.Add("CharacterSetComponent/Pants", new Tuple<string, string, List<long>, long>("Pants", "Change your character's pants.", _ped.Pants, _ped.PedComponents.Pants.Value));
            appearanceOptions.Add("CharacterSetComponent/Ponchos", new Tuple<string, string, List<long>, long>("Ponchos", "Change your character's poncho.", _ped.Ponchos, _ped.PedComponents.Poncho.Value));
            appearanceOptions.Add("CharacterSetComponent/Satchels", new Tuple<string, string, List<long>, long>("Satchels", "Change your character's satchels.", _ped.Ponchos, _ped.PedComponents.Satchel.Value));
            appearanceOptions.Add("CharacterSetComponent/Shirt", new Tuple<string, string, List<long>, long>("Shirt", "Change your character's shirt.", _ped.Shirts, _ped.PedComponents.Shirt.Value));

            if (!_ped.IsMale)
                appearanceOptions.Add("CharacterSetComponent/Skirts", new Tuple<string, string, List<long>, long>("Skirt", "Change your character's skirt.", _ped.Skirts, _ped.PedComponents.Skirt.Value));

            appearanceOptions.Add("CharacterSetComponent/Spats", new Tuple<string, string, List<long>, long>("Spats", "Change your character's spats.", _ped.Spats, _ped.PedComponents.Spats.Value));
            appearanceOptions.Add("CharacterSetComponent/Suspenders", new Tuple<string, string, List<long>, long>("Suspenders", "Change your character's suspenders.", _ped.Suspenders, _ped.PedComponents.Suspenders.Value));
            appearanceOptions.Add("CharacterSetComponent/Teeth", new Tuple<string, string, List<long>, long>("Teeth", "Change your character's teeth.", _ped.Teeth, _ped.PedComponents.Teeth.Value));
            appearanceOptions.Add("CharacterSetComponent/Vests", new Tuple<string, string, List<long>, long>("Vests", "Change your character's Vests.", _ped.Vests, _ped.PedComponents.Vest.Value));

            foreach(KeyValuePair<string, Tuple<string, string, List<long>, long>> kvp in appearanceOptions)
            {
                string endpoint = kvp.Key;
                string label = kvp.Value.Item1;
                string description = kvp.Value.Item2;
                List<long> list = kvp.Value.Item3;
                long currentValue = kvp.Value.Item4;
                MenuOptions menuOption = MenuOptions.MenuOptionList(label, description, endpoint, list, currentValue);
                moCharacterAppearance.AddOption(menuOption);
            }

            MenuOptions moCharacterSave = MenuOptions.MenuOptionButton("Confirm", "Confirm and save your character.", "CharacterConfirm");
            menuBase.AddOption(moCharacterSave);

            if (!update)
                Instance.NuiManager.Set("character/CREATOR", menuBase);
            else
                Instance.NuiManager.Patch("character/CREATOR", menuBase);

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
