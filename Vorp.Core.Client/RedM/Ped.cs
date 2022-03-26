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

        public void SetBodyComponent(PedComponent component)
        {
            Function.Call((Hash)0x1902C4CFCC5BE57C, Handle, (uint)component.Value);
        }

        public void SetComponent(PedComponent component)
        {
            if (component.Component == ePedComponentCategory.UNKNOWN) return;
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

            SetBodyComponent(comp.BodyType);
            SetBodyComponent(comp.BodyWaist);
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
            SetComponent(comp.Hat);
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

            while (VorpAPI.IsTextureValid(textureId))
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

            vorpComponents.BodyType.Value = BodyType[VorpAPI.Random.Next(BodyType.Count)];
            vorpComponents.BodyWaist.Value = BodyWaist[VorpAPI.Random.Next(BodyWaist.Count)];

            List<long> hair = Hairs;
            if (hair.Count > 0)
                vorpComponents.Hair.Value = hair[VorpAPI.Random.Next(hair.Count)];

            List<long> Teeth = CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.Teeth);
            if (!IsMale)
                Teeth = CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.Teeth);
            vorpComponents.Teeth.Value = Teeth[VorpAPI.Random.Next(Teeth.Count)];

            List<long> shirts = Shirts;
            if (shirts.Count > 0)
                vorpComponents.Shirt.Value = shirts[VorpAPI.Random.Next(shirts.Count)];

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

            List<long> boots = Boots;
            if (boots.Count > 0)
                vorpComponents.Boots.Value = boots[VorpAPI.Random.Next(boots.Count)];

            List<long> eyes = Eyes;
            if (eyes.Count > 0)
                vorpComponents.Eyes.Value = eyes[VorpAPI.Random.Next(eyes.Count)];

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

        public List<long> Eyes => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Eyes);
        public List<long> Heads => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Heads);
        public List<long> BodyType => CharacterComponentConfig.Config.Body.Types;
        public List<long> BodyWaist => CharacterComponentConfig.Config.Body.Waists;
        public List<long> BodiesUpper => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.BodiesUpper);
        public List<long> BodiesLower => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.BodiesLower);
        public List<long> Accessories => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Accessories);
        public List<long> Armor => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Armor);
        public List<long> Badges => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Badges);
        public List<long> Beards => CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.BeardsComplete);
        public List<long> Belts => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Belts);
        public List<long> BeltBuckles => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.BeltBuckles);
        public List<long> BootAccessories => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.BootAccessories);
        public List<long> Boots => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Boots);
        public List<long> Chaps => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Chaps);
        public List<long> Cloaks => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Cloaks);
        public List<long> Coats => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Coats);
        public List<long> CoatsClosed => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.CoatsClosed);
        public List<long> Dresses => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Dresses);
        public List<long> EyeWear => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.EyeWear);
        public List<long> Gauntlets => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Gauntlets);
        public List<long> Gloves => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Gloves);
        public List<long> Gunbelts => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Gunbelts);
        public List<long> GunbeltAccessories => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.GunbeltAccessories);
        public List<long> Hairs => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Hair);
        public List<long> HairAccessories => CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.HairAccessories);
        public List<long> Hats => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Hats);
        public List<long> HolstersLeft => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.HolstersLeft);
        public List<long> JewelryBracelets => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.JewelryBracelets);
        public List<long> JewelryRingsLeft => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.JewelryRingsLeft);
        public List<long> JewelryRingsRight => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.JewelryRingsRight);
        public List<long> Loadouts => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Loadouts);
        public List<long> Masks => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Masks);
        public List<long> Neckties => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Neckties);
        public List<long> Neckwear => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Neckwear);
        public List<long> Pants => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Pants);
        public List<long> Ponchos => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Ponchos);
        public List<long> Satchels => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Satchels);
        public List<long> Shirts => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.ShirtsFull);
        public List<long> Skirts => CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.Skirts);
        public List<long> Spats => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Spats);
        public List<long> Suspenders => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Suspenders);
        public List<long> Teeth => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Teeth);
        public List<long> Vests => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Vests);

        public List<long> CreatorAccessories => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Accessories, true);
        public List<long> CreatorArmor => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Armor, true);
        public List<long> CreatorBadges => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Badges, true);
        public List<long> CreatorBeards => CharacterComponentConfig.GetComponents(ePedType.Male, ePedComponentCategory.BeardsComplete, true);
        public List<long> CreatorBelts => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Belts, true);
        public List<long> CreatorBeltBuckles => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.BeltBuckles, true);
        public List<long> CreatorBootAccessories => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.BootAccessories, true);
        public List<long> CreatorBoots => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Boots, true);
        public List<long> CreatorChaps => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Chaps, true);
        public List<long> CreatorCloaks => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Cloaks, true);
        public List<long> CreatorCoats => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Coats, true);
        public List<long> CreatorCoatsClosed => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.CoatsClosed, true);
        public List<long> CreatorDresses => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Dresses, true);
        public List<long> CreatorEyeWear => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.EyeWear, true);
        public List<long> CreatorGauntlets => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Gauntlets, true);
        public List<long> CreatorGloves => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Gloves, true);
        public List<long> CreatorGunbelts => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Gunbelts, true);
        public List<long> CreatorGunbeltAccessories => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.GunbeltAccessories, true);
        public List<long> CreatorHairs => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Hair, true);
        public List<long> CreatorHairAccessories => CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.HairAccessories, true);
        public List<long> CreatorHats => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Hats, true);
        public List<long> CreatorHolstersLeft => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.HolstersLeft, true);
        public List<long> CreatorJewelryBracelets => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.JewelryBracelets, true);
        public List<long> CreatorJewelryRingsLeft => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.JewelryRingsLeft, true);
        public List<long> CreatorJewelryRingsRight => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.JewelryRingsRight, true);
        public List<long> CreatorLoadouts => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Loadouts, true);
        public List<long> CreatorMasks => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Masks, true);
        public List<long> CreatorNeckties => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Neckties, true);
        public List<long> CreatorNeckwear => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Neckwear, true);
        public List<long> CreatorPants => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Pants, true);
        public List<long> CreatorPonchos => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Ponchos, true);
        public List<long> CreatorSatchels => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Satchels, true);
        public List<long> CreatorShirts => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.ShirtsFull, true);
        public List<long> CreatorSkirts => CharacterComponentConfig.GetComponents(ePedType.Female, ePedComponentCategory.Skirts, true);
        public List<long> CreatorSpats => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Spats, true);
        public List<long> CreatorSuspenders => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Suspenders, true);
        public List<long> CreatorVests => CharacterComponentConfig.GetComponents(IsMale ? ePedType.Male : ePedType.Female, ePedComponentCategory.Vests, true);

        // TASKS
        public void TaskStartScenarioInPlace(string scenario, float heading, int duration = -1)
        {
            int hash = GetHashKey(scenario);
            Function.Call((Hash)0x524B54361229154F, Handle, (uint)hash, duration, true, false, heading, false);
        }

        public void ClearPedTasksImmediately(bool clearCrouch = true)
        {
            Function.Call((Hash)0xAAA34F8A7CB32098, Handle, true, clearCrouch);
        }
    }
}
