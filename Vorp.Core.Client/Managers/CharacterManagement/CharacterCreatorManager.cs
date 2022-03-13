using Vorp.Core.Client.Interface;
using Vorp.Shared.Models;

namespace Vorp.Core.Client.Managers.CharacterManagement
{
    public class CharacterCreatorManager : Manager<CharacterCreatorManager>
    {
        string _modelHashFemale = "mp_female";
        string _modelHashMale = "mp_male";
        Ped _pedMale;
        Ped _pedFemale;

        Camera _cameraMain;
        Camera _cameraMale;
        Camera _cameraFemale;

        Prompt _promptCharacter;
        Prompt _promptConfirm;
        bool _promptActive = false;

        bool _male = false;
        Ped _playerPed => Instance.LocalPlayer.Character;

        public override void Begin()
        {
            Instance.Hook("onResourceStop", new Action<string>(resourceName =>
            {
                if (GetCurrentResourceName() != resourceName) return;

                Dispose();
            }));
        }

        public async Task StartCharacterCreator()
        {
            await Screen.FadeOut(500);

            VorpAPI.StartSoloTutorialSession();
            await LoadImaps();
            await BaseScript.Delay(100);

            _playerPed.Position = new Vector3(-563.1345f, -3775.811f, 237.60f);
            _playerPed.IsPositionFrozen = true;

            await BaseScript.Delay(100);

            CreateCameras();
            CreatePrompts();

            World.SetWeather(Shared.Enums.eWeatherType.SUNNY);
            World.SetWeatherFrozen(true);
            World.WindSpeed = 0f;

            await BaseScript.Delay(100);
            _cameraMain.IsActive = true;
            SetCamera(CameraState.Main, _cameraMain);
            RenderScriptCams(true, false, 2000, true, true, 0);
            Instance.AttachTickHandler(OnWorldTime);

            await CreateSelections();
            await BaseScript.Delay(1000);
        }

        private async Task OnWorldTime()
        {
            Instance.WorldTime.ClockTimeOverride_2(7, 0, pauseClock: true);
        }

        private void CreatePrompts()
        {
            List<eControl> controls = new() { eControl.FrontendLeft, eControl.FrontendRight };

            _promptCharacter = Prompt.Create(controls, "Character");
            _promptConfirm = Prompt.Create(eControl.FrontendAccept, "Confirm", promptType: ePromptType.StandardHold);

            Instance.AttachTickHandler(OnPromptHandler);
        }

        private async Task OnPromptHandler()
        {
            if (_promptActive) return;
            _promptActive = true;

            if (_promptCharacter.IsJustPressed)
            {
                _male = API.IsControlPressed(0, (uint)eControl.FrontendLeft);
                await PromptCameraFemale_OnPromptEventsAsync();
            }
            else if (_promptConfirm.HasHoldModeCompleted)
                    await PromptConfirm_OnPromptEvents();

            _promptActive = false;
        }

        private async Task PromptConfirm_OnPromptEvents()
        {
            if (_promptConfirm.EventTriggered) return;
            _promptConfirm.EventTriggered = true;

            await Screen.FadeOut(500);

            string pedModel = "mp_female";
            VorpPedComponents comps = _pedFemale.PedComponents;

            if (_male)
            {
                pedModel = "mp_male";
                comps = _pedMale.PedComponents;
            }

            //VorpPlayer player = Instance.LocalPlayer;
            //player.SetModel(pedModel);
            //_playerPed.Position = new Vector3(-558.3258f, -3781.111f, 237.60f);
            //_playerPed.Heading = 93.2f;
            //_playerPed.IsPositionFrozen = true;
            //_playerPed.PedComponents = comps;

            CharacterEditorManager.Init(pedModel, comps);

            Dispose(false); // Need future feature to goback
        }

        private async Task PromptCameraFemale_OnPromptEventsAsync()
        {
            if (_cameraMain.IsActive && !_male)
                SetCamera(CameraState.SelectFemale, _cameraMain);
            else if (_cameraMain.IsActive && _male)
                SetCamera(CameraState.SelectMale, _cameraMain);
            else if (_cameraMale.IsActive)
                SetCamera(CameraState.SelectFemale, _cameraMale);
            else if (_cameraFemale.IsActive)
                SetCamera(CameraState.SelectMale, _cameraFemale);

            await BaseScript.Delay(2000);
        }

        private async Task LoadImaps()
        {
            await VorpAPI.GetImap(-1699673416);
            await VorpAPI.GetImap(1679934574);
            await VorpAPI.GetImap(183712523);
            Logger.Trace($"All IMAPs loaded");
        }

        async Task CreateSelections()
        {
            try
            {
                await CreateMalePed();
                await CreateFemalePed();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"CreateSelections");
            }
        }

        async Task CreateMalePed()
        {
            _pedMale = await VorpAPI.CreatePed(_modelHashMale, new Vector3(-558.52f, -3775.6f, 237.7f), 93.2f);
            _pedMale.IsPositionFrozen = true;
            SetEntityInvincible(_pedMale.Handle, true);
            RandomiseClothing(_pedMale);
        }

        async Task CreateFemalePed()
        {
            _pedFemale = await VorpAPI.CreatePed(_modelHashFemale, new Vector3(-558.43f, -3776.65f, 237.7f), 93.2f);
            _pedFemale.IsPositionFrozen = true;
            SetEntityInvincible(_pedFemale.Handle, true);
            RandomiseClothing(_pedFemale);
        }

        async void RandomiseClothing(Ped ped)
        {
            // I DO NOT KNOW WHY, I DO NOT WANT TO KNOW WHY
            // BUT I SUMISE, THAT IT IS DUE TO THE NATIVE NOT WORKING IN THE FIRST CALL
            // SO WE CALL IT TWICE
            ped.RandomiseClothingAsync();
            await BaseScript.Delay(0);
            ped.RandomiseClothingAsync();
        }

        void SetCamera(CameraState state, Camera previousCamera)
        {
            if (_cameraMain is null)
                CreateCameras();

            Camera activeCamera = null;
            if (state == CameraState.Main)
                activeCamera = _cameraMain;
            if (state == CameraState.SelectMale)
                activeCamera = _cameraMale;
            if (state == CameraState.SelectFemale)
                activeCamera = _cameraFemale;

            if (previousCamera != activeCamera)
            {
                SetCamActiveWithInterp(activeCamera.Handle, previousCamera.Handle, 2000, 250, 250);
                previousCamera.IsActive = false;
            }
        }

        enum CameraState
        {
            Main,
            SelectMale,
            SelectFemale
        }

        void CreateCameras()
        {
            Vector3 selctionRotation = new Vector3(0f, 0f, -91.93626f);
            float fov = 45.0f;

            _cameraMain = VorpAPI.CreateCameraWithParams(new Vector3(-561.4737f, -3776.209f, 239.1f), selctionRotation, fov);
            _cameraMale = VorpAPI.CreateCameraWithParams(new Vector3(-560.0516f, -3775.583f, 239.1f), selctionRotation, fov);
            _cameraFemale = VorpAPI.CreateCameraWithParams(new Vector3(-560.0867f, -3776.632f, 239.1f), selctionRotation, fov);
            _cameraMain.IsActive = false;
            _cameraMale.IsActive = false;
            _cameraFemale.IsActive = false;
        }

        void Dispose(bool resetTime = true)
        {
            Instance.DetachTickHandler(OnPromptHandler);
            Instance.DetachTickHandler(OnWorldTime);

            if (resetTime)
                Instance.WorldTime.ClearClockTimeOverride();

            _pedFemale.Delete();
            _pedMale.Delete();

            _promptConfirm.Delete();
            _promptCharacter.Delete();

            _cameraMain.Delete();
            _cameraMale.Delete();
            _cameraFemale.Delete();

            World.SetWeatherFrozen(false);
        }
    }
}
