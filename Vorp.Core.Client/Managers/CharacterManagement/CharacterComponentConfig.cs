using Vorp.Core.Client.Extensions;
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

        public static List<long> GetComponents(ePedType ePedType, ePedComponentCategory componentCategory)
        {
            try
            {
                List<long> components = new();

                if (ePedType == ePedType.Female && componentCategory == ePedComponentCategory.BeardsComplete)
                {
                    Logger.Warn($"Female peds cannot have Beards, blame R*.");
                    return components;
                }

                if (ePedType == ePedType.Male && (componentCategory == ePedComponentCategory.HairAccessories || componentCategory == ePedComponentCategory.Skirts))
                {
                    Logger.Warn($"Male ped models cannot have Skirts or Hair Accessories, blame R*.");
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
                        foreach (string comp in compCategory.Items)
                        {
                            long compValue = Convert.ToInt64(comp, 16);
                            components.Add(compValue);
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
    }
}
