using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace Vorp.Shared.Models
{
    [DataContract]
    public class Loadout
    {
        [DataMember(Name = "id")]
        [Description("id")]
        public int Id;

        [DataMember(Name = "identifier")]
        [Description("identifier")]
        public string Identifier;

        [DataMember(Name = "charidentifier")]
        [Description("charidentifier")]
        public int CharacterIdentifier;

        [DataMember(Name = "name")]
        [Description("name")]
        public string Name;

        [DataMember(Name = "ammo")]
        [Description("ammo")]
        public string Ammo;

        [DataMember(Name = "components")]
        [Description("components")]
        public string Components;

        [DataMember(Name = "dirtlevel")]
        [Description("dirtlevel")]
        public double Dirtlevel;

        [DataMember(Name = "mudlevel")]
        [Description("mudlevel")]
        public double Mudlevel;

        [DataMember(Name = "conditionlevel")]
        [Description("conditionlevel")]
        public double Conditionlevel;

        [DataMember(Name = "rustlevel")]
        [Description("rustlevel")]
        public double Rustlevel;

        [DataMember(Name = "used")]
        [Description("used")]
        public int Used;

        [DataMember(Name = "used2")]
        [Description("used")]
        public int Used2;

        [DataMember(Name = "dropped")]
        [Description("dropped")]
        public int Dropped;

        [DataMember(Name = "comps")]
        [Description("comps")]
        public string Comps;

        [DataMember(Name = "label")]
        [Description("label")]
        public string Label;
    }
}
