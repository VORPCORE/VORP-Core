using System.Runtime.Serialization;

namespace Vorp.Core.Server.Models
{
    [DataContract]
    public class Log
    {
        [DataMember(Name = "debug")]
        public bool Debug;

        [DataMember(Name = "warn")]
        public bool Warn;

        [DataMember(Name = "error")]
        public bool Error;
    }
}
