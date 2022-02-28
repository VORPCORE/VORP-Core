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

        public uint Value;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class VorpPedComponents
    {
        public PedComponent Hat = new PedComponent(ePedComponentCategory.Hats);
        public PedComponent EyeWear = new PedComponent(ePedComponentCategory.EyeWear);
        public PedComponent Mask = new PedComponent(ePedComponentCategory.Masks);
        public PedComponent NeckWear = new PedComponent(ePedComponentCategory.Neckwear);
        public PedComponent NeckTies = new PedComponent(ePedComponentCategory.Neckties);
        public PedComponent Shirt = new PedComponent(ePedComponentCategory.ShirtsFull);
        public PedComponent Suspender = new PedComponent(ePedComponentCategory.Suspenders);
        public PedComponent Vest = new PedComponent(ePedComponentCategory.Vests);
        public PedComponent Coat = new PedComponent(ePedComponentCategory.Coats);
        public PedComponent Poncho = new PedComponent(ePedComponentCategory.Ponchos);
        public PedComponent Cloak = new PedComponent(ePedComponentCategory.Cloaks);
        public PedComponent Glove = new PedComponent(ePedComponentCategory.Gloves);
        public PedComponent RingRh = new PedComponent(ePedComponentCategory.JewelryRingsRight);
        public PedComponent RingLh = new PedComponent(ePedComponentCategory.JewelryRingsLeft);
        public PedComponent Bracelet = new PedComponent(ePedComponentCategory.JewelryBracelets);
        public PedComponent Gunbelt = new PedComponent(ePedComponentCategory.Gunbelts);
        public PedComponent Belt = new PedComponent(ePedComponentCategory.Belts);
        public PedComponent Buckle = new PedComponent(ePedComponentCategory.BeltBuckles);
        public PedComponent Holster = new PedComponent(ePedComponentCategory.HolstersLeft);
        public PedComponent Pant = new PedComponent(ePedComponentCategory.Pants);
        public PedComponent Skirt = new PedComponent(ePedComponentCategory.Skirts);
        public PedComponent HairAccessories = new PedComponent(ePedComponentCategory.HairAccessories);
        public PedComponent Armor = new PedComponent(ePedComponentCategory.Armor);
        public PedComponent Teeth = new PedComponent(ePedComponentCategory.Teeth);
        public PedComponent Chap = new PedComponent(ePedComponentCategory.Chaps);
        public PedComponent Boots = new PedComponent(ePedComponentCategory.Boots);
        public PedComponent Spurs = new PedComponent(ePedComponentCategory.BootAccessories);
        public PedComponent Spats = new PedComponent(ePedComponentCategory.Spats);
        public PedComponent GunbeltAccs = new PedComponent(ePedComponentCategory.GunbeltAccessories);
        public PedComponent Gauntlets = new PedComponent(ePedComponentCategory.Gauntlets);
        public PedComponent Loadouts = new PedComponent(ePedComponentCategory.Loadouts);
        public PedComponent Accessories = new PedComponent(ePedComponentCategory.Accessories);
        public PedComponent Satchels = new PedComponent(ePedComponentCategory.Satchels);
        public PedComponent CoatClosed = new PedComponent(ePedComponentCategory.CoatsClosed);

        public override string ToString()
        {
            return JsonConvert.SerializeObject(Shirt);
        }
    }

    public class PedComponents
    {
        public long Hat;
        public long EyeWear;
        public long Mask;
        public long NeckWear;
        public long NeckTies;
        public long Shirt;
        public long Suspender;
        public long Vest;
        public long Coat;
        public long Poncho;
        public long Cloak;
        public long Glove;
        public long RingRh;
        public long RingLh;
        public long Bracelet;
        public long Gunbelt;
        public long Belt;
        public long Buckle;
        public long Holster;
        public long Pant;
        public long Skirt;
        public long bow;
        public long armor;
        public long teeth;
        public long Chap;
        public long Boots;
        public long Spurs;
        public long Spats;
        public long GunbeltAccs;
        public long Gauntlets;
        public long Loadouts;
        public long Accessories;
        public long Satchels;
        public long CoatClosed;
    }
}
