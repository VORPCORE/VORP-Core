using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vorp.Shared.Models
{
    [DataContract]
    public class ComponentCategory
    {
        [DataMember(Name = "category")]
        public string Category { get; set; }
        [DataMember(Name = "hash")]
        public string Hash { get; set; }
        [DataMember(Name = "items")]
        public List<string> Items { get; set; }
    }

    [DataContract]
    public class PedComponentOptions
    {
        [DataMember(Name = "female")]
        public List<ComponentCategory> Female { get; set; }
        [DataMember(Name = "horse")]
        public List<ComponentCategory> Horse { get; set; }
        [DataMember(Name = "male")]
        public List<ComponentCategory> Male { get; set; }
    }
}
