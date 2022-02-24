using System.Runtime.Serialization;

namespace Vorp.Core.Server.Models
{
    [DataContract]
    public class DatabaseConfig
    {
        [DataMember(Name = "server")]
        public string Server { get; set; }

        [DataMember(Name = "port")]
        public uint Port { get; set; } = 3306;

        [DataMember(Name = "database")]
        public string Database { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "minimumPoolSize")]
        public uint MinimumPoolSize { get; set; } = 10;

        [DataMember(Name = "maximumPoolSize")]
        public uint MaximumPoolSize { get; set; } = 50;

        [DataMember(Name = "connectionTimeout")]
        public uint ConnectionTimeout { get; set; } = 5;

    }
}
