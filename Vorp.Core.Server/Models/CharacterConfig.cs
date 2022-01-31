using System.Runtime.Serialization;

namespace Vorp.Core.Server.Models
{
    [DataContract]
    public class CharacterConfig
    {
        [DataMember(Name = "maximum")]
        public int Maximum;
        
        [DataMember(Name = "newCharacterGroup")]
        public string NewCharacterGroup;
    }
}
