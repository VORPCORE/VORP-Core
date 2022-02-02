namespace Vorp.Core.Server.Web.Discord.Entity
{
    public class EmbedAuthor
    {
        [JsonProperty(PropertyName = "name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name;

        [JsonProperty(PropertyName = "icon_url", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string IconUrl;
    }
}
