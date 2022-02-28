using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vorp.Shared.Models
{
    [DataContract]
    public class ComponentCategory
    {
        [DataMember(Name = "category")]
        public string Category;

        [DataMember(Name = "hash")]
        public string Hash;

        public long HashKey => Convert.ToInt64(Hash, 16);

        [DataMember(Name = "items")]
        public List<string> Items = new();
    }

    [DataContract]
    public class PedComponentOptions
    {
        [DataMember(Name = "female")]
        public List<ComponentCategory> Female = new();

        [DataMember(Name = "horse")]
        public List<ComponentCategory> Horse = new();

        [DataMember(Name = "male")]
        public List<ComponentCategory> Male = new();
    }
}
