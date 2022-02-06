using System.Runtime.Serialization;

namespace Vorp.Core.Client.Environment.Config
{
    [DataContract]
    public class PlayerNames
    {
        [DataMember(Name = "display")]
        public bool Display = true;

        [DataMember(Name = "displayId")]
        public bool DisplayId = true;

        [DataMember(Name = "distance")]
        public float Distance = 30;
    }
}
