using System.Runtime.Serialization;

namespace Vorp.Core.Server.Models
{
    [DataContract]
    public class Discord
    {
        [DataMember(Name = "botKey")]
        public string BotKey;

        [DataMember(Name = "botname")]
        public string Botname;

        [DataMember(Name = "webhooks")]
        public DiscordWebhooks Webhooks;

        [DataMember(Name = "whitelist")]
        public DiscordWhitelist Whitelist;

        [DataMember(Name = "guildId")]
        public ulong GuildId;

        [DataMember(Name = "url")]
        public string Url;
    }
}
