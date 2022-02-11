using System.Runtime.Serialization;

namespace Vorp.Core.Server.Models
{
    [DataContract]
    public class CharacterConfig
    {
        [DataMember(Name = "maximum")]
        public int Maximum = 2;

        [DataMember(Name = "init")]
        public InitiatedCharacter Init;
    }

    [DataContract]
    public class InitiatedCharacter
    {
        [DataMember(Name = "cash")]
        public double Cash;

        [DataMember(Name = "gold")]
        public double Gold;

        [DataMember(Name = "roleToken")]
        public double RoleToken;

        [DataMember(Name = "experience")]
        public int Experience;

        [DataMember(Name = "job")]
        public string Job;

        [DataMember(Name = "jobGrade")]
        public string JobGrade;

        [DataMember(Name = "group")]
        public string Group;
    }
}
