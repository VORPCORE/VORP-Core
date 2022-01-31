using System.Runtime.Serialization;

namespace Vorp.Core.Server.Models
{
    [DataContract]
    public class UserConfig
    {
        [DataMember(Name = "newUserGroup")]
        public string NewUserGroup;

        [DataMember(Name = "characters")]
        public CharacterConfig Characters;

    }
}
