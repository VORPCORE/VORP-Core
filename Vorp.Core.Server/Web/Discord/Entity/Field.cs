namespace Vorp.Core.Server.Web.Discord.Entity
{
    public class Field
    {
        [JsonProperty(PropertyName = "name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name;

        [JsonProperty(PropertyName = "value", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Value;

        [JsonProperty(PropertyName = "inline", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Inline;

        public Field(string name, string value, bool inline = false)
        {
            this.Name = name;
            this.Value = value;
            this.Inline = inline;
        }
    }
}
