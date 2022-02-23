using Vorp.Shared.Enums;

namespace Vorp.Shared.Models
{
    public class PedComponent
    {
        ePedComponent _component;

        public PedComponent(ePedComponent pedComponent)
        {
            _component = pedComponent;
        }

        public ePedComponent Component
        {
            get
            {
                return _component;
            }
        }

        public long Value;
    }

    public class VorpPedComponents
    {
        public PedComponent Hat = new PedComponent(ePedComponent.Hat);
        public PedComponent EyeWear = new PedComponent(ePedComponent.EyeWear);
        public PedComponent Mask = new PedComponent(ePedComponent.Mask);
        public PedComponent NeckWear = new PedComponent(ePedComponent.NeckWear);
        public PedComponent NeckTies = new PedComponent(ePedComponent.NeckTies);
        public PedComponent Shirt = new PedComponent(ePedComponent.Shirt);
        public PedComponent Suspender = new PedComponent(ePedComponent.Suspender);
        public PedComponent Vest = new PedComponent(ePedComponent.Vest);
        public PedComponent Coat = new PedComponent(ePedComponent.Coat);
        public PedComponent Poncho = new PedComponent(ePedComponent.Poncho);
        public PedComponent Cloak = new PedComponent(ePedComponent.Cloak);
        public PedComponent Glove = new PedComponent(ePedComponent.Glove);
        public PedComponent RingRh = new PedComponent(ePedComponent.RingRh);
        public PedComponent RingLh = new PedComponent(ePedComponent.RingLh);
        public PedComponent Bracelet = new PedComponent(ePedComponent.Bracelet);
        public PedComponent Gunbelt = new PedComponent(ePedComponent.Gunbelt);
        public PedComponent Belt = new PedComponent(ePedComponent.Belt);
        public PedComponent Buckle = new PedComponent(ePedComponent.Buckle);
        public PedComponent Holster = new PedComponent(ePedComponent.Holster);
        public PedComponent Pant = new PedComponent(ePedComponent.Pant);
        public PedComponent Skirt = new PedComponent(ePedComponent.Skirt);
        public PedComponent Bow = new PedComponent(ePedComponent.Bow);
        public PedComponent Armor = new PedComponent(ePedComponent.Armor);
        public PedComponent Teeth = new PedComponent(ePedComponent.Teeth);
        public PedComponent Chap = new PedComponent(ePedComponent.Chap);
        public PedComponent Boots = new PedComponent(ePedComponent.Boots);
        public PedComponent Spurs = new PedComponent(ePedComponent.Spurs);
        public PedComponent Spats = new PedComponent(ePedComponent.Spats);
        public PedComponent GunbeltAccs = new PedComponent(ePedComponent.GunbeltAccs);
        public PedComponent Gauntlets = new PedComponent(ePedComponent.Gauntlets);
        public PedComponent Loadouts = new PedComponent(ePedComponent.Loadouts);
        public PedComponent Accessories = new PedComponent(ePedComponent.Accessories);
        public PedComponent Satchels = new PedComponent(ePedComponent.Satchels);
        public PedComponent CoatClosed = new PedComponent(ePedComponent.CoatClosed);
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
