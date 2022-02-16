using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vorp.Core.Server.Models
{
    [DataContract]
    public class ServerConfig
    {
        string _whitelistType;
        List<string> _whitelistTypes = new()
        {
            "none",
            "discord",
            "database"
        };

        [DataMember(Name = "database")]
        public DatabaseConfig DatabaseConfig;

        [DataMember(Name = "language")]
        public string Language;

        [DataMember(Name = "whitelistType")]
        public string WhitelistType
        {
            get
            {
                return _whitelistType;
            }
            set
            {
                if (!_whitelistTypes.Contains(value))
                    Logger.Error($"Whitelist Type can only be, {string.Join(", ", _whitelistTypes)}.");

                _whitelistType = value;
            }
        }

        [DataMember(Name = "discord")]
        public Discord Discord;

        [DataMember(Name = "users")]
        public UserConfig UserConfig;
    }
}
