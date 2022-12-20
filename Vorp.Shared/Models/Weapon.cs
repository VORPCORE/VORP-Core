using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vorp.Shared.Models
{
    [DataContract]
    public class Weapon
    {
        [DataMember(Name = "id")]
        public int Id;

        [DataMember(Name = "name")]
        public string Name;

        [DataMember(Name = "propietary")]
        public string Propietary;

        [DataMember(Name = "ammo")]
        public Dictionary<string, int> Ammo = new();

        [DataMember(Name = "components")]
        public List<string> components = new();

        [DataMember(Name = "used")]
        public bool Used;

        [DataMember(Name = "used")]
        public bool Used2;

        [DataMember(Name = "charid")]
        public int CharacterId;
    }
}
