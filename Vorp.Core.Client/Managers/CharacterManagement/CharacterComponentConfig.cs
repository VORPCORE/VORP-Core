using Vorp.Shared.Enums;
using Vorp.Shared.Models;

namespace Vorp.Core.Client.Managers.CharacterManagement
{
    static class CharacterComponentConfig
    {
        static PedComponentOptions _pedComponentOptions;

        private static PedComponentOptions LoadConfiguration()
        {
            try
            {
                if (_pedComponentOptions is not null)
                    return _pedComponentOptions;

                string file = LoadResourceFile(GetCurrentResourceName(), $"/Resources/character-components.json");
                _pedComponentOptions = JsonConvert.DeserializeObject<PedComponentOptions>(file);

                return _pedComponentOptions;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Configuration was unable to be loaded.");
                return (PedComponentOptions)default!;
            }
        }

        public static PedComponentOptions Config => LoadConfiguration();
        public static List<ComponentCategory> Female => Config.Female;
        public static List<ComponentCategory> Male => Config.Male;
        public static List<ComponentCategory> Horse => Config.Horse;

        public static List<long> GetComponents(ePedType ePedType, ePedComponentCategory componentCategory, bool isCreator = false)
        {
            try
            {
                List<long> components = new();

                if (ePedType == ePedType.Female && componentCategory == ePedComponentCategory.BeardsComplete)
                {
                    return components;
                }

                if (ePedType == ePedType.Male && (componentCategory == ePedComponentCategory.HairAccessories || componentCategory == ePedComponentCategory.Skirts))
                {
                    return components;
                }

                List<ComponentCategory> componentCategories = new List<ComponentCategory>(Male);

                if (ePedType == ePedType.Female)
                    componentCategories = new List<ComponentCategory>(Female);
                if (ePedType == ePedType.Horse)
                    componentCategories = new List<ComponentCategory>(Horse);

                foreach (ComponentCategory compCategory in componentCategories)
                {
                    ePedComponentCategory category = (ePedComponentCategory)compCategory.HashKey;

                    if (category == componentCategory)
                    {
                        foreach (Component comp in compCategory.Items)
                        {
                            if (isCreator && comp.IsCreator)
                                components.Add(comp.HashKey);
                            else if (!isCreator)
                                components.Add(comp.HashKey);
                        }
                    }
                }

                return components;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error Getting Components");
                return new();
            }
        }

        public static TextureCategory GetRandomTextures(bool isMale)
        {
            List<TextureCategory> pedTextures = Config.Textures.Female;
            if (isMale)
                pedTextures = Config.Textures.Male;

            return pedTextures[VorpAPI.Random.Next(pedTextures.Count)];
        }

        public static TextureCategory GetPedTextures(bool isMale, uint selectedHead)
        {
            List<TextureCategory> pedTextures = Config.Textures.Female;
            if (isMale)
                pedTextures = Config.Textures.Male;

            foreach (TextureCategory tex in pedTextures)
            {
                if (tex.TextureHash == selectedHead)
                    return tex;
            }

            return null;
        }
    }
}
