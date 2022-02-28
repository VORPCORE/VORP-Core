using Vorp.Core.Client.Managers.CharacterManagement;
using Vorp.Shared.Enums;
using Vorp.Shared.Models;

namespace Vorp.Core.Client.RedM
{
    public class Ped : Entity
    {
        private VorpPedComponents _vorpPedComponents;

        public Ped(int handle) : base(handle)
        {

        }

        public bool IsMale => IsPedMale(Handle);
        public void SetPedOutfitPreset(int preset) => Function.Call((Hash)0x77FF8D35EEC6BBC4, Handle, preset, 0);
        public bool IsPedReadyToRender => Function.Call<bool>((Hash)0xA0BC8FAED8CFEB3C, Handle);
        public void RandomOutfitVariation()
        {
            Function.Call((Hash)0x283978A15512B2FE, Handle, true);
        }

        public void BodyComponent(PedComponent component)
        {
            Function.Call((Hash)0x1902C4CFCC5BE57C, Handle, component.Value);
            UpdatePedVariation();
        }

        public void ApplyShopItemToPed(long componentHash, bool immediately = true, bool isMultiplayer = true, bool p4 = false)
        {
            Function.Call((Hash)0xD3A7B003ED343FD9, Handle, componentHash, immediately, isMultiplayer, p4);
            UpdatePedVariation();
        }

        public void ApplyShopItemToPed(PedComponent component)
        {
            ApplyShopItemToPed(component.Value);
            UpdatePedVariation();
        }

        public void RemoveTagFromMetaPed(uint component, int p2 = 0) => Function.Call((Hash)0xD710A5007C2AC539, Handle, component, p2);

        public async void ApplyDefaultSkinSettings()
        {
            while (!IsPedReadyToRender)
            {
                await BaseScript.Delay(0);
            }

            Function.Call((Hash)0x0BFA1BD465CDFEFD, Handle);

            List<long> Eyes = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.Eyes);
            if (!IsMale)
                Eyes = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.Eyes);

            List<long> Heads = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.Heads);
            if (!IsMale)
                Heads = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.Heads);

            List<long> BodiesUpper = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.BodiesUpper);
            if (!IsMale)
                BodiesUpper = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.BodiesUpper);

            List<long> BodiesLower = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.BodiesLower);
            if (!IsMale)
                BodiesLower = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.BodiesLower);

            long compEyes = Eyes[VorpAPI.Random.Next(Eyes.Count)];
            long compHead = Heads[VorpAPI.Random.Next(Heads.Count)];
            long compBody = BodiesUpper[VorpAPI.Random.Next(BodiesUpper.Count)];
            long compLegs = BodiesLower[VorpAPI.Random.Next(BodiesLower.Count)];

            ApplyShopItemToPed(compEyes);
            ApplyShopItemToPed(compHead);
            ApplyShopItemToPed(compBody);
            ApplyShopItemToPed(compLegs);

            RemoveTagFromMetaPed(0x1D4C528A, 0);
            RemoveTagFromMetaPed(0x3F1F01E5, 0);
            RemoveTagFromMetaPed(0xDA0E2C55, 0);
            UpdatePedVariation();
        }

        public void UpdatePedVariation(bool p1 = false, bool p5 = false) => Function.Call((Hash)0xCC8CA3E88256E58F, Handle, p1, true, true, true, p5);

        public void SetComponent(PedComponent component)
        {
            Function.Call((Hash)0x59BD177A1A48600A, Handle, (uint)component.Component);
            ApplyShopItemToPed(component);
        }

        public void RandomiseClothingAsync()
        {
            if (_vorpPedComponents is not null) return;

            VorpPedComponents vorpComponents = new VorpPedComponents();

            List<long> Hair = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.Hair);
            if (!IsMale)
                Hair = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.Hair);
            if (Hair.Count > 0)
            {
                vorpComponents.Hair.Value = Hair[VorpAPI.Random.Next(Hair.Count)];
                SetComponent(vorpComponents.Hair);
                UpdatePedVariation(true);
            }

            List<long> Teeth = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.Teeth);
            if (!IsMale)
                Teeth = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.Teeth);
            vorpComponents.Teeth.Value = Teeth[VorpAPI.Random.Next(Teeth.Count)];
            SetComponent(vorpComponents.Teeth);
            UpdatePedVariation(true);

            List<long> Shirts = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.ShirtsFull);
            if (!IsMale)
                Shirts = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.ShirtsFull);

            List<long> Boots = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.Boots);
            if (!IsMale)
                Boots = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.Boots);
            if (Shirts.Count > 0)
            {
                vorpComponents.Shirt.Value = Shirts[VorpAPI.Random.Next(Shirts.Count)];
                SetComponent(vorpComponents.Shirt);
                UpdatePedVariation(true);
            }

            if (IsMale)
            {
                List<long> Beards = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.BeardsComplete);
                if (Beards.Count > 0 && VorpAPI.Random.Next(3) == 1)
                {
                    vorpComponents.Beard.Value = Beards[VorpAPI.Random.Next(Beards.Count)];
                    SetComponent(vorpComponents.Beard);
                    UpdatePedVariation(true);
                }

                List<long> MalePants = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.Pants);
                if (MalePants.Count > 0)
                {
                    vorpComponents.Pants.Value = MalePants[VorpAPI.Random.Next(MalePants.Count)];
                    SetComponent(vorpComponents.Pants);
                    UpdatePedVariation(true);
                }
            }

            if (!IsMale)
            {
                List<long> HairAccessories = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.HairAccessories);
                if (HairAccessories.Count > 0)
                {
                    vorpComponents.HairAccessory.Value = HairAccessories[VorpAPI.Random.Next(HairAccessories.Count)];
                    SetComponent(vorpComponents.HairAccessory);
                    UpdatePedVariation(true);
                }

                List<long> FemaleSkirts = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.Skirts);
                if (FemaleSkirts.Count > 0)
                {
                    vorpComponents.Skirt.Value = FemaleSkirts[VorpAPI.Random.Next(FemaleSkirts.Count)];
                    SetComponent(vorpComponents.Skirt);
                    UpdatePedVariation(true);
                }
            }

            if (Boots.Count > 0)
            {
                vorpComponents.Boots.Value = Boots[VorpAPI.Random.Next(Boots.Count)];
                SetComponent(vorpComponents.Boots);
                UpdatePedVariation(true);
            }

            _vorpPedComponents = vorpComponents;
        }
    }
}
