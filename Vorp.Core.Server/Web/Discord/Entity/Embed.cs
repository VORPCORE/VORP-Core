using System.Collections.Generic;

namespace Vorp.Core.Server.Web.Discord.Entity
{
    public class Embed
    {
        [JsonProperty(PropertyName = "author", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public EmbedAuthor Author;

        [JsonProperty(PropertyName = "title", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Title;

        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description;

        [JsonProperty(PropertyName = "color", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Color;

        [JsonProperty(PropertyName = "thumbnail", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public EmbedThumbnail Thumbnail;

        [JsonProperty(PropertyName = "fields", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<Field> fields = new List<Field>();
    }
}
