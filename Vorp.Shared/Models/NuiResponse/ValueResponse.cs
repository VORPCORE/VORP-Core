using System.Runtime.Serialization;

namespace Vorp.Shared.Models.NuiResponse
{
    [DataContract]
    public class ValueResponse
    {
        [DataMember(Name = "args")]
        public List<string> Arguments = new();

        public Dictionary<string, dynamic> GetArgmentAtIndex(int idx)
        {
            if (Arguments.Count < idx) return null;
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Arguments[idx]);
        }
    }
}
