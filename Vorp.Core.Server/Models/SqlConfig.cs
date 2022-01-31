using System.Runtime.Serialization;

namespace Vorp.Core.Server.Models
{
    [DataContract]
    public class SqlConfig
    {
        [DataMember(Name = "type")]
        public string Type;

        [DataMember(Name = "username")]
        public string Username;

        [DataMember(Name = "password")]
        public string Password;

        [DataMember(Name = "database")]
        public string Database;

        [DataMember(Name = "server")]
        public string Server;

        [DataMember(Name = "port")]
        public int Port;

        public override string ToString()
        {
            switch (Type)
            {
                case "mysql":
                    return $"server={Server}:{Port};user={Username};password={Password};database={Database}";
                case "mariadb":
                    return $"mysql://{Username}/{Password}:@{Server}:{Port}/{Database}";
                default:
                    return $"Unknown Type '{Type}'";
            }
        }
    }
}
