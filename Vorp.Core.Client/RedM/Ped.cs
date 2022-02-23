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
        public void ApplyShopItemToPed(uint componentHash, bool immediately = true, bool isMultiplayer = true, bool p4 = true) => Function.Call((Hash)0xD3A7B003ED343FD9, Handle, componentHash, immediately, isMultiplayer, p4);
        public void ApplyShopItemToPed(PedComponent component, bool immediately = true, bool isMultiplayer = true, bool p4 = true) => Function.Call((Hash)0xD3A7B003ED343FD9, Handle, component.Value, immediately, isMultiplayer, p4);
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
            VorpPedComponents vorpCompoents = new VorpPedComponents();
            vorpCompoents.Hat.Value = 0;
            vorpCompoents.EyeWear.Value = 0;
            vorpCompoents.Mask.Value = 0;
            vorpCompoents.NeckWear.Value = 0;
            vorpCompoents.NeckTies.Value = 0;
            vorpCompoents.Shirt.Value = VorpAPI.Random.Next(1, 5);
            vorpCompoents.Suspender.Value = 0;
            vorpCompoents.Vest.Value = 0;
            vorpCompoents.Coat.Value = 0;
            vorpCompoents.Poncho.Value = 0;
            vorpCompoents.Cloak.Value = 0;
            vorpCompoents.Glove.Value = 0;
            vorpCompoents.RingRh.Value = 0;
            vorpCompoents.RingLh.Value = 0;
            vorpCompoents.Bracelet.Value = 0;
            vorpCompoents.Gunbelt.Value = 0;
            vorpCompoents.Belt.Value = 0;
            vorpCompoents.Buckle.Value = 0;
            vorpCompoents.Holster.Value = 0;
            
            if (IsPedMale(Handle))
                vorpCompoents.Pant.Value = VorpAPI.Random.Next(1, 5);

            if (!IsPedMale(Handle)) 
                vorpCompoents.Skirt.Value = VorpAPI.Random.Next(1, 5);

            vorpCompoents.Bow.Value = 0;
            vorpCompoents.Armor.Value = 0;
            vorpCompoents.Teeth.Value = 0;
            vorpCompoents.Chap.Value = 0;
            vorpCompoents.Boots.Value = VorpAPI.Random.Next(1, 5);
            vorpCompoents.Spurs.Value = 0;
            vorpCompoents.Spats.Value = 0;
            vorpCompoents.GunbeltAccs.Value = 0;
            vorpCompoents.Gauntlets.Value = 0;
            vorpCompoents.Loadouts.Value = 0;
            vorpCompoents.Accessories.Value = 0;
            vorpCompoents.Satchels.Value = 0;
            vorpCompoents.CoatClosed.Value = 0;

            Type type = vorpCompoents.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach(PropertyInfo property in properties)
            {
                Logger.Trace($"Property: {property.Name}");
                var prop = property.GetValue(vorpCompoents, null);
                SetComponent((PedComponent)prop);
            }
        }
    }
}
