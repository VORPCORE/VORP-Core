using Vorp.Core.Client.Interface;
using Vorp.Core.Client.RedM.Enums;
using Vorp.Shared.Records;

namespace Vorp.Core.Client.Managers.CharacterManagement
{
    public class CharacterManager : Manager<CharacterManager>
    {
        public bool IsInCharacterSelection = false;
        string _modelHashFemale = "mp_female";
        string _modelHashMale = "mp_male";
        Ped _pedMale;
        Ped _pedFemale;

        bool isSelectingSex = true;

        Camera CameraMain;
        Camera CameraMale;
        Camera CameraFemale;

        public override void Begin()
        {
            Instance.Hook("onResourceStop", new Action<string>(resourceName =>
            {
                if (GetCurrentResourceName() != resourceName) return;

                Dispose();
            }));

            ClientGateway.Mount("vorp:character:list", new Action<Dictionary<int, Character>, int>(async (characters, maxCharcters) =>
            {
                IsInCharacterSelection = true;
                Logger.Trace($"Received {characters.Count} characters from the server, max {maxCharcters} allowed.");

                await Screen.FadeOut(500);

                DisplayHud(false);
                DisplayRadar(false);

                await LoadImaps();
                Instance.LocalPlayer.Position = new Vector3(-563.1345f, -3775.811f, 237.60f);
                Instance.LocalPlayer.Ped.IsPositionFrozen = true;

                await BaseScript.Delay(100);

                CreateCameras();
                await BaseScript.Delay(100);
                CameraMain.IsActive = true;
                SetCamera(CameraState.Main, CameraMain);
                RenderScriptCams(true, true, 2000, true, true, 0);

                await CreateSelections();

                if (characters.Count == 0)
                {
                    Logger.Trace($"No characters, lets goto character creator.");
                    return;
                }

                foreach (var kvp in characters)
                {
                    int characterId = kvp.Key;
                    Character character = kvp.Value;
                }

                await Screen.FadeIn(500);
            }));
        }

        private async Task LoadImaps()
        {
            await GetImap(-1699673416);
            await GetImap(1679934574);
            await GetImap(183712523);
            Logger.Trace($"All IMAPs loaded");
        }

        async Task CreateSelections()
        {
            try
            {
                isSelectingSex = true;

                Instance.AttachTickHandler(OnCameraControls);
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
            _pedMale.ApplyDefaultSkinSettings();
            _pedMale.IsPositionFrozen = true;
            SetEntityInvincible(_pedMale.Handle, true);
            RandomiseClothing(_pedMale);
        }

        async Task CreateFemalePed()
        {
            _pedFemale = await VorpAPI.CreatePed(_modelHashFemale, new Vector3(-558.43f, -3776.65f, 237.7f), 93.2f);
            _pedFemale.ApplyDefaultSkinSettings();
            _pedFemale.IsPositionFrozen = true;
            SetEntityInvincible(_pedFemale.Handle, true);
            RandomiseClothing(_pedFemale);
        }

        void RandomiseClothing(Ped ped)
        {
            // I DO NOT KNOW WHY, I DO NOT WANT TO KNOW WHY
            // BUT I SUMISE, THAT IT IS DUE TO THE NATIVE NOT WORKING IN THE FIRST CALL
            // SO WE CALL IT TWICE
            ped.RandomiseClothing();
            ped.UpdatePedVariation();
            ped.RandomiseClothing();
            ped.UpdatePedVariation();
        }

        async Task GetImap(int hash)
        {
            while (!Function.Call<bool>(Hash._IS_IMAP_ACTIVE, hash))
            {
                Function.Call(Hash._REQUEST_IMAP, hash);
                await BaseScript.Delay(100);
            }
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
            Instance.DetachTickHandler(OnCameraControls);

            CameraMain.Delete();
            CameraMale.Delete();
            CameraFemale.Delete();
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
                activeCamera= CameraFemale;

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

        private async Task OnCameraControls()
        {
            if (isSelectingSex)
            {
                if (IsControlJustPressed(2, (uint)eControls.FrontendRight))
                {
                    if (CameraMain.IsActive)
                        SetCamera(CameraState.SelectFemale, CameraMain);
                    else if (CameraMale.IsActive)
                        SetCamera(CameraState.SelectFemale, CameraMale);
                }

                if (IsControlJustPressed(2, (uint)eControls.FrontendLeft))
                {
                    if (CameraMain.IsActive)
                        SetCamera(CameraState.SelectMale, CameraMain);
                    else if (CameraFemale.IsActive)
                        SetCamera(CameraState.SelectMale, CameraFemale);
                }

                if (IsControlJustPressed(2, (uint)eControls.FrontendAccept))
                {
                    if (CameraMale.IsActive)
                        SetCamera(CameraState.Main, CameraMale);
                    else if (CameraFemale.IsActive)
                        SetCamera(CameraState.Main, CameraFemale);
                }
            }
        }
    }
}
