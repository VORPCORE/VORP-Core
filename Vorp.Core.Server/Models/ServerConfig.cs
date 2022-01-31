using System.Runtime.Serialization;

namespace Vorp.Core.Server.Models
{
    [DataContract]
    public class ServerConfig
    {
        [DataMember(Name = "log")]
        public Log Log;

        [DataMember(Name = "discord")]
        public Discord Discord;

        [DataMember(Name = "sql")]
        public SqlConfig SqlConfig;

        [DataMember(Name = "users")]
        public UserConfig UserConfig;
    }
}
