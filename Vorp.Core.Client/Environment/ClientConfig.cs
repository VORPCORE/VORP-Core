using System.Runtime.Serialization;
using Vorp.Core.Client.Environment.Config;

namespace Vorp.Core.Client.Environment
{
    [DataContract]
    public class ClientConfig
    {
        [DataMember(Name = "language")]
        public Language Language;

        [DataMember(Name = "pvp")]
        public bool PvpEnabled;

        [DataMember(Name = "playerNames")]
        public PlayerNames PlayerNames;

        [DataMember(Name = "discord")]
        public DiscordSettings Discord;
    }

    [DataContract]
    public class Language
    {
        [DataMember(Name = "defaultLanguage")]
        public string DefaultLanguage;

        [DataMember(Name = "languages")]
        public List<string> Languages;
    }
}
