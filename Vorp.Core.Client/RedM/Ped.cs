using System.Reflection;
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

        public void ApplyShopItemToPed(uint componentHash, bool immediately = true, bool isMultiplayer = true, bool p4 = true)
        {
            Function.Call((Hash)0xD3A7B003ED343FD9, Handle, componentHash, immediately, isMultiplayer, p4);
            UpdatePedVariation();
        }
        
        public void ApplyShopItemToPed(PedComponent component)
        {
            ApplyShopItemToPed(component.Value, p4: !IsMale);
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
            List<uint> ShirtMale = new()
            {
                0x00215112,
                0x015BDA7D,
                0x021A1A7A,
                0x04F54118,
                0x0567DD7D,
            };

            List<uint> ShirtFemale = new()
            {
                0x004869A5,
                0x007CDF56,
                0x01C0A6E7,
                0x03671B0A,
                0x04446738,
            };

            int rand = VorpAPI.Random.Next(1, 5);

            VorpPedComponents vorpComponents = new VorpPedComponents();
            vorpComponents.Shirt.Value = IsMale ? ShirtMale[rand] : ShirtFemale[rand];
            SetComponent(vorpComponents.Shirt);
            UpdatePedVariation(true);

            if (IsMale)
            {
                vorpComponents.Pant.Value = 0x010051C7;
                SetComponent(vorpComponents.Pant);
                UpdatePedVariation(true);
            }

            if (!IsMale)
            {
                vorpComponents.Skirt.Value = 180955894;
                SetComponent(vorpComponents.Skirt);
                UpdatePedVariation(true);
            }

            vorpComponents.Boots.Value = IsMale ? 0x9F3252BB : 0x019ADA9E;
            SetComponent(vorpComponents.Boots);
            UpdatePedVariation(true);

            _vorpPedComponents = vorpComponents;
            Logger.Trace($"Sex: {IsMale} / Comps: {vorpComponents.Shirt.Value}");
        }
    }
}
