using Vorp.Core.Client.Environment.Entities;
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

        Camera CameraMain;
        Camera CameraMale;
        Camera CameraFemale;

        Prompt PromptCharacter;
        Prompt PromptConfirm;
        bool promptActive = false;

        bool male = false;

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

            Instance.LocalPlayer.Position = new Vector3(-563.1345f, -3775.811f, 237.60f);
            Instance.LocalPlayer.IsPositionFrozen = true;

            await BaseScript.Delay(100);

            CreateCameras();
            CreatePrompts();

            await BaseScript.Delay(100);
            CameraMain.IsActive = true;
            SetCamera(CameraState.Main, CameraMain);
            RenderScriptCams(true, true, 2000, true, true, 0);

            await CreateSelections();
        }

        private void CreatePrompts()
        {
            List<eControl> controls = new() { eControl.FrontendLeft, eControl.FrontendRight };

            PromptCharacter = Prompt.Create(controls, "Character");
            PromptConfirm = Prompt.Create(eControl.FrontendAccept, "Confirm", promptType: ePromptType.StandardHold);

            Instance.AttachTickHandler(OnPromptHandler);
        }

        private async Task OnPromptHandler()
        {
            if (promptActive) return;
            promptActive = true;

            if (PromptCharacter.IsJustPressed)
            {
                male = API.IsControlPressed(0, (uint)eControl.FrontendLeft);
                await PromptCameraFemale_OnPromptEventsAsync();
            }
            else if (PromptConfirm.HasHoldModeCompleted)
                    await PromptConfirm_OnPromptEvents();

            promptActive = false;
        }

        private async Task PromptConfirm_OnPromptEvents()
        {
            if (PromptConfirm.EventTriggered) return;
            PromptConfirm.EventTriggered = true;

            await Screen.FadeOut(500);

            string playerModel = "mp_female";
            VorpPedComponents comps = _pedFemale.PedComponents;

            if (CameraMale.IsActive)
            {
                playerModel = "mp_male";
                comps = _pedMale.PedComponents;
            }

            VorpPlayer player = Instance.LocalPlayer;
            await player.SetModel(playerModel);
            Vector3 playerCreationPosition = new Vector3(-558.3258f, -3781.111f, 237.60f);
            await player.Teleport(playerCreationPosition);
            player.Heading = 93.2f;
            player.IsPositionFrozen = true;
            player.Ped.PedComponents = comps;

            CharacterEditor.Init();

            Dispose();
        }

        private async Task PromptCameraFemale_OnPromptEventsAsync()
        {
            if (CameraMain.IsActive && !male)
                SetCamera(CameraState.SelectFemale, CameraMain);
            else if (CameraMain.IsActive && male)
                SetCamera(CameraState.SelectMale, CameraMain);
            else if (CameraMale.IsActive)
                SetCamera(CameraState.SelectFemale, CameraMale);
            else if (CameraFemale.IsActive)
                SetCamera(CameraState.SelectMale, CameraFemale);

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
                Instance.AttachTickHandler(FreezeClock);

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
            _pedMale.ApplyRandomBodySettings();
            _pedMale.IsPositionFrozen = true;
            SetEntityInvincible(_pedMale.Handle, true);
            RandomiseClothing(_pedMale);
        }

        async Task CreateFemalePed()
        {
            _pedFemale = await VorpAPI.CreatePed(_modelHashFemale, new Vector3(-558.43f, -3776.65f, 237.7f), 93.2f);
            _pedFemale.ApplyRandomBodySettings();
            _pedFemale.IsPositionFrozen = true;
            SetEntityInvincible(_pedFemale.Handle, true);
            RandomiseClothing(_pedFemale);
        }

        async void RandomiseClothing(Ped ped)
        {
            // I DO NOT KNOW WHY, I DO NOT WANT TO KNOW WHY
            // BUT I SUMISE, THAT IT IS DUE TO THE NATIVE NOT WORKING IN THE FIRST CALL
            // SO WE CALL IT TWICE
            ped.ApplyRandomSkinSettings();
            ped.UpdatePedVariation();
            await BaseScript.Delay(0);
            ped.ApplyRandomSkinSettings();
            ped.UpdatePedVariation();
        }

        void SetCamera(CameraState state, Camera previousCamera)
        {
            if (CameraMain is null)
                CreateCameras();

            Camera activeCamera = null;
            if (state == CameraState.Main)
                activeCamera = CameraMain;
            if (state == CameraState.SelectMale)
                activeCamera = CameraMale;
            if (state == CameraState.SelectFemale)
                activeCamera = CameraFemale;

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

        private async Task FreezeClock()
        {
            NetworkClockTimeOverride(18, 0, 0, 0, true);
            SetClockTime(18, 0, 0);
            PauseClock(true, 0);
        }

        void CreateCameras()
        {
            Vector3 selctionRotation = new Vector3(0f, 0f, -91.93626f);
            float fov = 45.0f;

            CameraMain = VorpAPI.CreateCameraWithParams(new Vector3(-561.4737f, -3776.209f, 239.1f), selctionRotation, fov);
            CameraMale = VorpAPI.CreateCameraWithParams(new Vector3(-560.0516f, -3775.583f, 239.1f), selctionRotation, fov);
            CameraFemale = VorpAPI.CreateCameraWithParams(new Vector3(-560.0867f, -3776.632f, 239.1f), selctionRotation, fov);
        }

        void Dispose()
        {
            if (_pedFemale is not null)
                _pedFemale.Delete();

            if (_pedMale is not null)
                _pedMale.Delete();

            Instance.DetachTickHandler(FreezeClock);
            Instance.DetachTickHandler(OnPromptHandler);

            PromptConfirm.Delete();
            PromptCharacter.Delete();

            CameraMain.Delete();
            CameraMale.Delete();
            CameraFemale.Delete();
        }
    }
}
