using System.Runtime.Serialization;

namespace Vorp.Core.Server.Models
{
    [DataContract]
    public class DiscordWhitelist
    {
        [DataMember(Name = "enabled")]
        public bool Enabled;

        [DataMember(Name = "role")]
        public ulong Role;
    }
}