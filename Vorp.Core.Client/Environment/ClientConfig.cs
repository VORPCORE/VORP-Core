using System.Runtime.Serialization;
using Vorp.Core.Client.Environment.Config;

namespace Vorp.Core.Client.Environment
{
    [DataContract]
    public class ClientConfig
    {
        [DataMember(Name = "playerNames")]
        public PlayerNames PlayerNames;

        [DataMember(Name = "discord")]
        public DiscordSettings Discord;
    }
}
