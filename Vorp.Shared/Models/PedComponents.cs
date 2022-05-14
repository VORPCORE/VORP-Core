using Vorp.Shared.Enums;

namespace Vorp.Shared.Models
{
    public class PedComponent
    {
        ePedComponentCategory _component;

        public PedComponent(ePedComponentCategory pedComponent)
        {
            _component = pedComponent;
        }

        public ePedComponentCategory Component
        {
            get
            {
                return _component;
            }
        }

        public long Value = 0;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class VorpPedComponents
    {
        public PedComponent Accessory = new PedComponent(ePedComponentCategory.Accessories);
        public PedComponent Armor = new PedComponent(ePedComponentCategory.Armor);
        public PedComponent Badge = new PedComponent(ePedComponentCategory.Badges);
        public PedComponent Beard = new PedComponent(ePedComponentCategory.BeardsComplete);
        public PedComponent Belt = new PedComponent(ePedComponentCategory.Belts);
        public PedComponent BeltBuckle = new PedComponent(ePedComponentCategory.BeltBuckles);
        public PedComponent BodyType = new PedComponent(ePedComponentCategory.BodyType);
        public PedComponent BodyWaist = new PedComponent(ePedComponentCategory.BodyWaist);
        public PedComponent BodyUpper = new PedComponent(ePedComponentCategory.BodiesUpper);
        public PedComponent BodyLower = new PedComponent(ePedComponentCategory.BodiesLower);
        public PedComponent Boots = new PedComponent(ePedComponentCategory.Boots);
        public PedComponent BootAccessory = new PedComponent(ePedComponentCategory.BootAccessories);
        public PedComponent Chaps = new PedComponent(ePedComponentCategory.Chaps);
        public PedComponent Cloak = new PedComponent(ePedComponentCategory.Cloaks);
        public PedComponent Coat = new PedComponent(ePedComponentCategory.Coats);
        public PedComponent CoatClosed = new PedComponent(ePedComponentCategory.CoatsClosed);
        public PedComponent Dresses = new PedComponent(ePedComponentCategory.Dresses);
        public PedComponent Eyes = new PedComponent(ePedComponentCategory.Eyes);
        public PedComponent EyeWear = new PedComponent(ePedComponentCategory.EyeWear);
        public PedComponent Gauntlet = new PedComponent(ePedComponentCategory.Gauntlets);
        public PedComponent Gloves = new PedComponent(ePedComponentCategory.Gloves);
        public PedComponent Gunbelt = new PedComponent(ePedComponentCategory.Gunbelts);
        public PedComponent GunbeltAccessory = new PedComponent(ePedComponentCategory.GunbeltAccessories);
        public PedComponent Hair = new PedComponent(ePedComponentCategory.Hair);
        public PedComponent HairAccessory = new PedComponent(ePedComponentCategory.HairAccessories);
        public PedComponent Hat = new PedComponent(ePedComponentCategory.Hats);
        public PedComponent Head = new PedComponent(ePedComponentCategory.Heads);
        public PedComponent HolstersLeft = new PedComponent(ePedComponentCategory.HolstersLeft);
        public PedComponent JewelryBracelets = new PedComponent(ePedComponentCategory.JewelryBracelets);
        public PedComponent JewelryRingsLeft = new PedComponent(ePedComponentCategory.JewelryRingsLeft);
        public PedComponent JewelryRingsRight = new PedComponent(ePedComponentCategory.JewelryRingsRight);
        public PedComponent Loadout = new PedComponent(ePedComponentCategory.Loadouts);
        public PedComponent Masks = new PedComponent(ePedComponentCategory.Masks);
        public PedComponent Neckties = new PedComponent(ePedComponentCategory.Neckties);
        public PedComponent Neckwear = new PedComponent(ePedComponentCategory.Neckwear);
        public PedComponent Pants = new PedComponent(ePedComponentCategory.Pants);
        public PedComponent Poncho = new PedComponent(ePedComponentCategory.Ponchos);
        public PedComponent Satchel = new PedComponent(ePedComponentCategory.Satchels);
        public PedComponent Shirt = new PedComponent(ePedComponentCategory.ShirtsFull);
        public PedComponent Skirt = new PedComponent(ePedComponentCategory.Skirts);
        public PedComponent Spats = new PedComponent(ePedComponentCategory.Spats);
        public PedComponent Suspenders = new PedComponent(ePedComponentCategory.Suspenders);
        public PedComponent Teeth = new PedComponent(ePedComponentCategory.Teeth);
        public PedComponent Vest = new PedComponent(ePedComponentCategory.Vests);

        public long Texture = GetHashKey("mp_head_fr1_sc08_c0_000_ab");
        public long Normal = GetHashKey("mp_head_mr1_000_nm");
        public long Material = 0x7FC5B1E1;
        public int SkinTone;
        public float Opacity;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(Shirt);
        }
    }
}
