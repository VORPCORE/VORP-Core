using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vorp.Core.Server.Models
{
    [DataContract]
    public class SqlConfig
    {
        private string _databaseType;
        private List<string> _databaseTypes = new()
        {
            "mysql",
            "mariadb"
        };

        [DataMember(Name = "type")]
        public string Type
        {
            get { return _databaseType; }
            set
            {
                if (!_databaseTypes.Contains(value))
                    Logger.Error($"Unknown Database Type: must be, {string.Join(", ", _databaseTypes)}");

                _databaseType = value;
            }
        }

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
