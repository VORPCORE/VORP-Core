using Vorp.Core.Client.Interface;
using Vorp.Shared.Records;

namespace Vorp.Core.Client.Managers.CharacterManagement
{
    public class CharacterManager : Manager<CharacterManager>
    {
        string _modelHashFemale = "mp_female";
        string _modelHashMale = "mp_male";
        Ped _pedMale;
        Ped _pedFemale;

        public override void Begin()
        {
            Instance.Hook("onResourceStop", new Action<string>(resourceName =>
            {
                if (GetCurrentResourceName() != resourceName) return;

                if (_pedFemale is not null)
                    _pedFemale.Delete();

                if (_pedMale is not null)
                    _pedMale.Delete();

                Instance.DetachTickHandler(FreezeClock);
            }));

            ClientGateway.Mount("vorp:character:list", new Action<Dictionary<int, Character>, int>(async (characters, maxCharcters) =>
            {
                Logger.Trace($"Received {characters.Count} characters from the server, max {maxCharcters} allowed.");

                await Screen.FadeOut(500);

                await GetImap(-1699673416);
                await GetImap(1679934574);
                await GetImap(183712523);

                Logger.Trace($"All IMAPs loaded");

                Instance.LocalPlayer.Position = new Vector3(-563.1345f, -3775.811f, 237.60f);

                Instance.AttachTickHandler(FreezeClock);

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
            NetworkClockTimeOverride(12, 0, 0, 0, true);
            SetClockTime(12, 0, 0);
            PauseClock(true, 0);
        }
    }
}
