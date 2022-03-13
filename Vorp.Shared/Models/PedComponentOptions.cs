using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vorp.Shared.Models
{
    [DataContract]
    public class Component
    {
        [DataMember(Name = "hash")]
        public string Hash;

        [DataMember(Name = "isCreator")]
        public bool IsCreator;

        public long HashKey => Convert.ToInt64(Hash, 16);
    }

    [DataContract]
    public class ComponentCategory
    {
        [DataMember(Name = "category")]
        public string Category;

        [DataMember(Name = "hash")]
        public string Hash;

        public long HashKey => Convert.ToInt64(Hash, 16);

        [DataMember(Name = "items")]
        public List<Component> Items = new();
    }

    [DataContract]
    public class TextureCategory
    {
        [DataMember(Name = "texture")]
        public string Texture;

        [DataMember(Name = "head")]
        public List<string> Head = new();

        [DataMember(Name = "body")]
        public List<string> Body = new();

        [DataMember(Name = "leg")]
        public List<string> Leg = new();

        public long TextureHash => GetHashKey(Texture);

        public List<long> Heads
        {
            get
            {
                List<long> result = new();
                foreach (var item in Head)
                {
                    long compValue = Convert.ToInt64(item, 16);
                    result.Add(compValue);
                }
                return result;
            }
        }
        public List<long> BodiesUpper
        {
            get
            {
                List<long> result = new();
                foreach (var item in Body)
                {
                    long compValue = Convert.ToInt64(item, 16);
                    result.Add(compValue);
                }
                return result;
            }
        }
        public List<long> BodiesLower
        {
            get
            {
                List<long> result = new();
                foreach (var item in Leg)
                {
                    long compValue = Convert.ToInt64(item, 16);
                    result.Add(compValue);
                }
                return result;
            }
        }
    }

    [DataContract]
    public class PedTextures
    {
        [DataMember(Name = "female")]
        public List<TextureCategory> Female = new();

        [DataMember(Name = "male")]
        public List<TextureCategory> Male = new();
    }

    [DataContract]
    public class PedComponentOptions
    {
        [DataMember(Name = "textures")]
        public PedTextures Textures = new();

        [DataMember(Name = "female")]
        public List<ComponentCategory> Female = new();

        [DataMember(Name = "horse")]
        public List<ComponentCategory> Horse = new();

        [DataMember(Name = "male")]
        public List<ComponentCategory> Male = new();
    }
}
