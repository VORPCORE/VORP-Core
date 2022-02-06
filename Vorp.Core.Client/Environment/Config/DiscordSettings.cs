using System.Runtime.Serialization;
using Vorp.Core.Client.Environment.Config.Discord;

namespace Vorp.Core.Client.Environment.Config
{
    [DataContract]
    public class DiscordSettings
    {
        [DataMember(Name = "appId")]
        public ulong AppId;

        [DataMember(Name = "asset")]
        public Asset Asset;
    }
}
