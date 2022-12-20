using System.Runtime.Serialization;

namespace Vorp.Core.Client.Environment.Config.Discord
{
    [DataContract]
    public class Asset
    {
        [DataMember(Name = "title")]
        public string Title;

        [DataMember(Name = "description")]
        public string Description;

        [DataMember(Name = "icon")]
        public string Icon;

        [DataMember(Name = "iconText")]
        public string IconText;
    }
}
