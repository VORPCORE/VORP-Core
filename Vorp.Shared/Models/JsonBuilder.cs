using System.Collections.Generic;
using System.Linq;

namespace Vorp.Shared.Models
{
    public class JsonBuilder
    {
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        public JsonBuilder Add(string key, object value)
        {
            Data.Add(key, value);

            return this;
        }

        public JsonBuilder Remove(string key)
        {
            Data.Remove(key);

            return this;
        }

        public T Find<T>(string key)
        {
            Data.TryGetValue(key, out var result);

            return result != null ? JsonConvert.DeserializeObject<T>(result.ToString()) : default(T);
        }

        public string Build()
        {
            return JsonConvert.SerializeObject(Data);
        }

        public string BuildCollection()
        {
            return JsonConvert.SerializeObject(Data.Select(self => self.Value));
        }

        public List<object> ToCollection()
        {
            return Data.Select(self => self.Value).ToList();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(Data);
        }
    }
}
