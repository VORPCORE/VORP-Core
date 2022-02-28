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

        public void ApplyShopItemToPed(uint componentHash, bool immediately = true, bool isMultiplayer = true, bool p4 = false)
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

            uint compEyes = 612262189;
            uint compHead = 0x206061DB;
            uint compBody = 0xA0BE4A7B;
            uint compLegs = 0x84BAA309;

            if (!IsPedMale(Handle))
            {
                compEyes = 928002221;
                compHead = 0x489AFE52;
                compBody = 0x76ACA91E;
                compLegs = 0x11A244CC;
            }

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

        public void RandomiseClothing()
        {
            List<uint> Shirts = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.ShirtsFull);
            if (!IsMale)
                Shirts = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.ShirtsFull);

            List<uint> Boots = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.Boots);
            if (!IsMale)
                Boots = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.Boots);

            VorpPedComponents vorpComponents = new VorpPedComponents();
            if (Shirts.Count > 0)
            {
                vorpComponents.Shirt.Value = Shirts[VorpAPI.Random.Next(Shirts.Count)];
                SetComponent(vorpComponents.Shirt);
                UpdatePedVariation(true);
            }

            if (IsMale)
            {
                List<uint> MalePants = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.Pants);
                if (MalePants.Count > 0)
                {
                    vorpComponents.Pant.Value = MalePants[VorpAPI.Random.Next(MalePants.Count)];
                    SetComponent(vorpComponents.Pant);
                    UpdatePedVariation(true);
                }
            }

            if (!IsMale)
            {
                List<uint> FemaleSkirts = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.Pants);
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
            Logger.Trace($"Sex: {IsMale} / Comps: {vorpComponents.Shirt.Value}");
        }
    }
}
