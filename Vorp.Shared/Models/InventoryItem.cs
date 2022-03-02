using System.Runtime.Serialization;

namespace Vorp.Shared.Models
{
    [DataContract]
    public class InventoryItem
    {
        [DataMember(Name = "label")]
        public string Label;

        [DataMember(Name = "name")]
        public string Name;

        [DataMember(Name = "type")]
        public string Type;

        [DataMember(Name = "count")]
        public int Count;

        [DataMember(Name = "limit")]
        public int Limit;

        [DataMember(Name = "usable")]
        public bool Usable;

        [DataMember(Name = "canRemove")]
        public bool IsRemovable;
    }
}
