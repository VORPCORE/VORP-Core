using System.Collections.Generic;
using Vorp.Shared.Models;
using Vorp.Shared.Records;

namespace Vorp.Core.Server.Managers.Legacy
{
    public class LegacyInventoryManager : Manager<LegacyInventoryManager>
    {
        public static Dictionary<string, CallbackDelegate> UseableItemCallbacks = new();

        public override void Begin()
        {
            Event("vorpinventory:getInventory", new Action<Player>(OnGetInventory));
        }

        private async void OnGetInventory([FromSource] Player player)
        {
            try
            {
                User user = PluginManager.ToUser(player.Handle);
                if (user == null) return;
                Dictionary<string, InventoryItem> inventory = await user.ActiveCharacter.GetInventoryItems();
                user.Player.TriggerEvent("vorpInventory:giveInventory", inventory);

                List<Loadout> loadouts = await user.ActiveCharacter.GetDatabaseLoadout();
                user.Player.TriggerEvent("vorpInventory:giveLoadout", loadouts);
            }
            catch (Exception ex)
            {
                Logger.Error($"OnGetInventory: {ex.Message}");
            }
        }
    }
}
