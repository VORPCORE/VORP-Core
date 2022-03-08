using Vorp.Core.Client.Interface;
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
        public bool IsPlayer => IsPedAPlayer(Handle);
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

        public void UpdatePedVariation(bool p1 = false, bool p5 = false) => Function.Call((Hash)0xCC8CA3E88256E58F, Handle, p1, true, true, true, p5);

        public void SetComponent(PedComponent component)
        {
            if (component.Component == ePedComponentCategory.Unknown) return;
            if (component.Value == 0)
                RemoveTagFromMetaPed((uint)component.Component, 0);
            else
                Function.Call((Hash)0x59BD177A1A48600A, Handle, (uint)component.Component);
            ApplyShopItemToPed(component);
        }

        public VorpPedComponents PedComponents
        {
            get => _vorpPedComponents;
            set
            {
                _vorpPedComponents = value;
                SetupComponents();
            }
            
        }

        public void UpdatePedTexture(int textureId) => Function.Call((Hash)0x92DAABA2C1C10B0E, textureId);

        public void ApplyTexture(long component, int textureId) => Function.Call((Hash)0x0B46E25761519058, Handle, component, textureId);

        public void UpdateComponents() => SetupComponents();

        private async void SetupComponents()
        {
            while (!IsPedReadyToRender)
            {
                await BaseScript.Delay(0);
            }

            Function.Call((Hash)0x0BFA1BD465CDFEFD, Handle);

            VorpPedComponents comp = _vorpPedComponents;

            ApplyShopItemToPed(comp.Eyes);
            UpdatePedVariation();
            ApplyShopItemToPed(comp.Head);
            UpdatePedVariation();
            ApplyShopItemToPed(comp.BodyUpper);
            UpdatePedVariation();
            ApplyShopItemToPed(comp.BodyLower);
            UpdatePedVariation();

            RemoveTagFromMetaPed(0x1D4C528A, 0);
            RemoveTagFromMetaPed(0x3F1F01E5, 0);
            RemoveTagFromMetaPed(0xDA0E2C55, 0);
            UpdatePedVariation();

            // Comps
            SetComponent(comp.Accessory);
            SetComponent(comp.Armor);
            SetComponent(comp.Badge);
            SetComponent(comp.Beard);
            SetComponent(comp.Belt);
            SetComponent(comp.BeltBuckle);
            SetComponent(comp.BootAccessory);
            SetComponent(comp.Boots);
            SetComponent(comp.Chaps);
            SetComponent(comp.Cloak);
            SetComponent(comp.Coat);
            SetComponent(comp.CoatClosed);
            SetComponent(comp.Dresses);
            SetComponent(comp.EyeWear);
            SetComponent(comp.Gauntlet);
            SetComponent(comp.Gloves);
            SetComponent(comp.Gunbelt);
            SetComponent(comp.GunbeltAccessory);
            SetComponent(comp.Hair);
            SetComponent(comp.HairAccessory);
            SetComponent(comp.Hats);
            SetComponent(comp.HolstersLeft);
            SetComponent(comp.JewelryBracelets);
            SetComponent(comp.JewelryRingsLeft);
            SetComponent(comp.JewelryRingsRight);
            SetComponent(comp.Loadout);
            SetComponent(comp.Masks);
            SetComponent(comp.Neckties);
            SetComponent(comp.Neckwear);
            SetComponent(comp.Pants);
            SetComponent(comp.Poncho);
            SetComponent(comp.Satchel);
            SetComponent(comp.Shirt);
            SetComponent(comp.Skirt);
            SetComponent(comp.Spats);
            SetComponent(comp.Suspenders);
            SetComponent(comp.Teeth);
            SetComponent(comp.Vest);
            UpdatePedVariation(true);

            // Overlays
            int textureId = VorpAPI.RequestTexture(comp.Texture, comp.Normal, comp.Material);

            while(VorpAPI.IsTextureValid(textureId))
            {
                await BaseScript.Delay(0);
            }

            ApplyTexture(GetHashKey("heads"), textureId);
            UpdatePedTexture(textureId);
        }

        public void RandomiseClothingAsync(bool forceRandomise = false)
        {
            if (_vorpPedComponents is not null && !forceRandomise) return;

            Function.Call((Hash)0x0BFA1BD465CDFEFD, Handle);

            VorpPedComponents vorpComponents = new VorpPedComponents();

            List<long> hair = Hairs;
            if (hair.Count > 0)
                vorpComponents.Hair.Value = hair[VorpAPI.Random.Next(hair.Count)];

            List<long> Teeth = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.Teeth);
            if (!IsMale)
                Teeth = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.Teeth);
            vorpComponents.Teeth.Value = Teeth[VorpAPI.Random.Next(Teeth.Count)];

            List<long> Shirts = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.ShirtsFull);
            if (!IsMale)
                Shirts = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.ShirtsFull);

            List<long> Boots = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.Boots);
            if (!IsMale)
                Boots = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.Boots);
            if (Shirts.Count > 0)
                vorpComponents.Shirt.Value = Shirts[VorpAPI.Random.Next(Shirts.Count)];

            if (IsMale)
            {
                List<long> beards = Beards;
                if (beards.Count > 0 && VorpAPI.Random.Next(3) == 1)
                    vorpComponents.Beard.Value = beards[VorpAPI.Random.Next(beards.Count)];

                List<long> MalePants = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.Pants);
                if (MalePants.Count > 0)
                    vorpComponents.Pants.Value = MalePants[VorpAPI.Random.Next(MalePants.Count)];
            }

            if (!IsMale)
            {
                List<long> HairAccessories = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.HairAccessories);
                if (HairAccessories.Count > 0)
                    vorpComponents.HairAccessory.Value = HairAccessories[VorpAPI.Random.Next(HairAccessories.Count)];

                List<long> FemaleSkirts = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.Skirts);
                if (FemaleSkirts.Count > 0)
                    vorpComponents.Skirt.Value = FemaleSkirts[VorpAPI.Random.Next(FemaleSkirts.Count)];
            }

            if (Boots.Count > 0)
                vorpComponents.Boots.Value = Boots[VorpAPI.Random.Next(Boots.Count)];

           List <long> eyes = Eyes;
            if (eyes.Count > 0)
                vorpComponents.Eyes.Value = Eyes[VorpAPI.Random.Next(Eyes.Count)];

            TextureCategory textureCategory = CharacterComponentConfig.GetRandomTextures(IsMale);
            if (textureCategory != null)
            {
                vorpComponents.Texture = textureCategory.TextureHash;
            }

            List<long> Heads = textureCategory.Heads;
            List<long> BodiesUpper = textureCategory.BodiesUpper;
            List<long> BodiesLower = textureCategory.BodiesLower;
            
            vorpComponents.Head.Value = Heads[VorpAPI.Random.Next(Heads.Count)];
            vorpComponents.BodyUpper.Value = BodiesUpper[VorpAPI.Random.Next(BodiesUpper.Count)];
            vorpComponents.BodyLower.Value = BodiesLower[VorpAPI.Random.Next(BodiesLower.Count)];

            PedComponents = vorpComponents;
        }

        internal async Task Teleport(Vector3 pos, bool withFade = true, bool findGround = true)
        {
            if (withFade) await Screen.FadeOut(500);
            float groundZ = pos.Z;
            Vector3 norm = pos;

            IsPositionFrozen = true;

            if (API.GetGroundZAndNormalFor_3dCoord(pos.X, pos.Y, pos.Z, ref groundZ, ref norm) && findGround)
                norm = new Vector3(pos.X, pos.Y, groundZ);

            Position = norm;
            IsPositionFrozen = false;

            if (withFade) await Screen.FadeIn(500);
        }

        public bool IsCollisionEnabled
        {
            get => !GetEntityCollisionDisabled(Handle);
            set => SetEntityCollision(Handle, value, value);
        }

        public bool CanRagdoll
        {
            get => CanPedRagdoll(Handle);
            set => SetPedCanRagdoll(Handle, value);
        }

        public bool IsVisible
        {
            get => IsEntityVisible(Handle);
            set => SetEntityVisible(Handle, value);
        }

        public int Opacity
        {
            get => GetEntityAlpha(Handle);
            set => SetEntityAlpha(Handle, value, false);
        }

        public bool IsDead => IsEntityDead(Handle);

        public List<long> Heads => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Heads);
        public List<long> Eyes => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Eyes);
        public List<long> Hairs => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Hair);
        public List<long> Beards => CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.BeardsComplete);
        public List<long> BodiesUpper => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.BodiesUpper);
        public List<long> BodiesLower => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.BodiesLower);
    }
}
