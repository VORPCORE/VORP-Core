using System.Reflection;
using Vorp.Shared.Models;

namespace Vorp.Core.Client.RedM
{
    public class Ped : Entity
    {
        public Ped(int handle) : base(handle)
        {

        }

        public void SetPedOutfitPreset(int preset) => Function.Call((Hash)0x77FF8D35EEC6BBC4, Handle, preset, 0);
        public bool IsPedReadyToRender => Function.Call<bool>((Hash)0xA0BC8FAED8CFEB3C, Handle);
        public void RandomComponentVariation() => Function.Call(Hash.SET_PED_RANDOM_COMPONENT_VARIATION, Handle, 1);

        public async void BodyComponent(PedComponent component)
        {
            Function.Call((Hash)0x1902C4CFCC5BE57C, Handle, component.Value);
            await BaseScript.Delay(0);
            UpdatePedVariation();
        }

        public void ApplyShopItemToPed(uint componentHash, bool immediately = true, bool isMultiplayer = true, bool p4 = true) => Function.Call((Hash)0xD3A7B003ED343FD9, Handle, componentHash, immediately, isMultiplayer, p4);
        
        public async void ApplyShopItemToPed(PedComponent component, bool immediately = true, bool isMultiplayer = true, bool p4 = true)
        {
            Function.Call((Hash)0xD3A7B003ED343FD9, Handle, component.Value, immediately, isMultiplayer, p4);
            await BaseScript.Delay(0);
            UpdatePedVariation();
        }
        
        public void RemoveTagFromMetaPed(uint component, int p2 = 0) => Function.Call((Hash)0xD710A5007C2AC539, Handle, component, p2);

        public async void ApplyDefaultSkinSettings()
        {
            SetPedOutfitPreset(1);

            while (!IsPedReadyToRender)
            {
                await BaseScript.Delay(0);
            }

            Function.Call((Hash)0x0BFA1BD465CDFEFD, Handle);

            uint compEyes = 612262189;
            uint compBody = 0xA0BE4A7B;
            uint compHead = 0x206061DB;
            uint compLegs = 0x84BAA309;

            if (!IsPedMale(Handle))
            {
                compEyes = 928002221;
                compBody = 0x76ACA91E;
                compHead = 0x489AFE52;
                compLegs = 0x11A244CC;
            }

            ApplyShopItemToPed(compHead);
            ApplyShopItemToPed(compEyes);
            ApplyShopItemToPed(compBody);
            ApplyShopItemToPed(compLegs);

            RemoveTagFromMetaPed(0x1D4C528A, 0);
            RemoveTagFromMetaPed(0x3F1F01E5, 0);
            RemoveTagFromMetaPed(0xDA0E2C55, 0);
            UpdatePedVariation();
        }

        public void UpdatePedVariation() => Function.Call((Hash)0xCC8CA3E88256E58F, Handle, false, true, true, true, false);

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

            bool isMale = IsPedMale(Handle);
            int rand = VorpAPI.Random.Next(1, 5);

            VorpPedComponents vorpCompoents = new VorpPedComponents();
            vorpCompoents.Shirt.Value = isMale ? ShirtMale[rand] : ShirtFemale[rand];
            BodyComponent(vorpCompoents.Shirt);

            if (IsPedMale(Handle))
            {
                vorpCompoents.Pant.Value = 0x010051C7;
                SetComponent(vorpCompoents.Pant);
            }

            if (!IsPedMale(Handle))
            {
                vorpCompoents.Skirt.Value = 4726031;
                SetComponent(vorpCompoents.Skirt);
            }

            vorpCompoents.Boots.Value = isMale ? (uint)0x38B4CA64 : 0x019ADA9E;
            SetComponent(vorpCompoents.Boots);
        }
    }
}
