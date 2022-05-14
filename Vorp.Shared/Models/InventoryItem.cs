using System.ComponentModel;
using System.Runtime.Serialization;

#if SERVER
using Vorp.Core.Server.Database;
using System.Threading.Tasks;
using Dapper;
#endif

namespace Vorp.Shared.Models
{
    [DataContract]
    public class InventoryItem
    {
        [Description("item")]
        public string Item;

        [DataMember(Name = "label")]
        [Description("label")]
        public string Label;

        // need to deprecate this
        [DataMember(Name = "name")]
        public string Name
        {
            get { return Label; }
        }

        [DataMember(Name = "type")]
        [Description("type")]
        public string Type;

        [DataMember(Name = "count")]
        public int Count;

        [DataMember(Name = "limit")]
        [Description("limit")]
        public int Limit;

        [DataMember(Name = "usable")]
        [Description("usable")]
        public bool Usable;

        [DataMember(Name = "canRemove")]
        [Description("can_remove")]
        public bool IsRemovable;

#if SERVER
        public static async Task<InventoryItem> GetItem(string item)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("item", item);
            InventoryItem inventoryItem = await DapperDatabase<InventoryItem>.GetSingleAsync("select * from `items` where `item` = @item", dynamicParameters);
            Common.MoveToMainThread();
            return inventoryItem;
        }
#endif
    }
}
