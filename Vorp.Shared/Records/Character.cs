﻿#if SERVER
using Dapper;
using Vorp.Core.Server.Database;
using System.Threading.Tasks;
using Vorp.Core.Server;
#endif

using System.Collections.Generic;
using System.ComponentModel;
using Vorp.Shared.Models;

namespace Vorp.Shared.Records
{
    public partial class Character
    {
        // I DON'T KNOW WHY, I DON'T WANT TO KNOW WHY, BUT, IF YOU CAN FIX IT, MAY THE GENERATOR SEE IN YOUR FAVOUR!

        #region Fields
        [Description("identifier")]
        public string SteamIdentifier { get; set; } = default!;

        [Description("steamname")]
        public string SteamName { get; set; } = default!;

        [Description("charidentifier")]
        public int CharacterId { get; set; }

        [Description("group")]
        public string Group { get; private set; } = "user";

        [Description("money")]
        public double Cash { get; private set; } = 0.00;

        [Description("gold")]
        public double Gold { get; private set; } = 0.00;

        [Description("rol")]
        public double RoleToken { get; private set; } = 0.00;

        [Description("xp")]
        public int Experience { get; private set; } = 0;

        // inventory should not be a string right now, it should be the output of a class
        [Description("inventory")]
        public string Inventory { get; set; } = "{}";

        [Description("job")]
        public string Job { get; private set; } = "unemployed";

        [Description("status")]
        public string Status { get; set; } = "{}";

        [Description("meta")]
        public string Meta { get; set; } = "{}";

        [Description("firstname")]
        public string Firstname { get; set; }

        [Description("lastname")]
        public string Lastname { get; set; }

        [Description("skinPlayer")]
        public string Skin { get; set; } = "{}";

        [Description("compPlayer")]
        public string Components { get; set; } = "{}";

        [Description("jobgrade")]
        public int JobGrade { get; set; } = 0;

        [Description("coords")]
        public string Coords { get; set; } = "{}";

        [Description("isdead")]
        public bool IsDead { get; set; } = false;

        [Description("clanid")]
        public int ClanId { get; set; } = 0;

        [Description("trust")]
        public int Trust { get; set; } = 0;

        [Description("supporter")]
        public int Supporter { get; set; } = 0;

        [Description("walk")]
        public string Walk { get; set; } = "noanim";

        [Description("crafting")]
        public string Crafting { get; set; } = "{}";

        [Description("info")]
        public string Info { get; set; } = "{}";

        [Description("gunsmith")]
        public double GunSmith { get; private set; } = 0.00;

        #endregion

        #region Properties
        public bool IsActive { get; set; } = false;

        public string Fullname => $"{Firstname} {Lastname}";
        #endregion

        #region Methods

#if SERVER
        // TODO: Move all SQL into procedures? or EF after refactor?
        internal async Task<bool> AdjustCurrency(bool increase, int currency, double amount)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("characterId", CharacterId);

                double amt = 0;

                switch (currency)
                {
                    case 0:
                        amt = increase ? amount : amount * -1;
                        dynamicParameters.Add("money", amt);
                        Cash = await DapperDatabase<double>.GetSingleAsync($"UPDATE characters SET `money` += @money WHERE `charIdentifier` = @characterId; SELECT `money` from characters WHERE `charIdentifier` = @characterId;", dynamicParameters);
                        break;
                    case 1:
                        amt = increase ? amount : amount * -1;
                        dynamicParameters.Add("gold", amt);
                        Gold = await DapperDatabase<double>.GetSingleAsync($"UPDATE characters SET `gold` += @gold WHERE `charIdentifier` = @characterId; SELECT `gold` from characters WHERE `charIdentifier` = @characterId;", dynamicParameters);
                        break;
                    case 2:
                        amt = increase ? amount : amount * -1;
                        dynamicParameters.Add("roleToken", amt);
                        RoleToken = await DapperDatabase<double>.GetSingleAsync($"UPDATE characters SET `rol` += @roleToken WHERE `charIdentifier` = @characterId; SELECT `rol` from characters WHERE `charIdentifier` = @characterId;", dynamicParameters);
                        break;
                    default:
                        PluginManager.Logger.Error($"Character.AdjustCurrency {currency} value is unknown");
                        return false;
                }
                await Common.MoveToMainThread();
                return true;
            }
            catch (Exception ex)
            {
                PluginManager.Logger.Error("AdjustCurrency");
                PluginManager.Logger.Error(ex.Message);
                return false;
            }
        }

        internal async Task<bool> AdjustExperience(bool increase, int experience)
        {
            try
            {
                int amt = increase ? experience : experience * -1;
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("characterId", CharacterId);
                dynamicParameters.Add("experience", amt);
                Experience = await DapperDatabase<int>.GetSingleAsync($"UPDATE characters SET `xp` += @experience WHERE `charIdentifier` = @characterId; SELECT `xp` from characters WHERE `charIdentifier` = @characterId;", dynamicParameters);
                await Common.MoveToMainThread();
                return true;
            }
            catch (Exception ex)
            {
                PluginManager.Logger.Error("AdjustExperience");
                PluginManager.Logger.Error(ex.Message);
                return false;
            }
        }

        internal async Task<bool> SetJob(string job)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("characterId", CharacterId);
                dynamicParameters.Add("job", job);
                Job = await DapperDatabase<string>.GetSingleAsync($"UPDATE characters SET `job` = @job WHERE `charIdentifier` = @characterId; SELECT `job` from characters WHERE `charIdentifier` = @characterId;", dynamicParameters);
                await Common.MoveToMainThread();
                return true;
            }
            catch (Exception ex)
            {
                PluginManager.Logger.Error("SetJob");
                PluginManager.Logger.Error(ex.Message);
                return false;
            }
        }

        internal async Task<bool> SetJobGrade(int grade)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("characterId", CharacterId);
                dynamicParameters.Add("grade", grade);
                Job = await DapperDatabase<string>.GetSingleAsync($"UPDATE characters SET `jobgrade` = @grade WHERE `charIdentifier` = @characterId; SELECT `job` from characters WHERE `charIdentifier` = @characterId;", dynamicParameters);
                await Common.MoveToMainThread();
                return true;
            }
            catch (Exception ex)
            {
                PluginManager.Logger.Error("SetJobGrade");
                PluginManager.Logger.Error(ex.Message);
                return false;
            }
        }

        internal async Task<bool> SetJobAndGrade(string job, int grade)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("characterId", CharacterId);
                dynamicParameters.Add("job", job);
                dynamicParameters.Add("grade", grade);
                Job = await DapperDatabase<string>.GetSingleAsync($"UPDATE characters SET `job` = @job, `jobgrade` = @grade WHERE `charIdentifier` = @characterId; SELECT `job` from characters WHERE `charIdentifier` = @characterId;", dynamicParameters);
                await Common.MoveToMainThread();
                return true;
            }
            catch (Exception ex)
            {
                PluginManager.Logger.Error("SetJobAndGrade");
                PluginManager.Logger.Error(ex.Message);
                return false;
            }
        }

        internal async Task<bool> SetGroup(string group)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("characterId", CharacterId);
                dynamicParameters.Add("group", group);
                Group = await DapperDatabase<string>.GetSingleAsync($"UPDATE characters SET `group` = @group WHERE `charIdentifier` = @characterId; SELECT `group` from characters WHERE `charIdentifier` = @characterId;", dynamicParameters);
                await Common.MoveToMainThread();
                return true;
            }
            catch (Exception ex)
            {
                PluginManager.Logger.Error("SetGroup");
                PluginManager.Logger.Error(ex.Message);
                return false;
            }
        }

        internal async Task<bool> SetDead(bool isDead)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("characterId", CharacterId);
                dynamicParameters.Add("dead", isDead);
                int result = await DapperDatabase<int>.GetSingleAsync($"UPDATE characters SET `isdead` = @dead WHERE `charIdentifier` = @characterId; SELECT `isdead` from characters WHERE `charIdentifier` = @characterId;", dynamicParameters);
                await Common.MoveToMainThread();
                return result > 0; // if result is true, then user is dead
            }
            catch (Exception ex)
            {
                PluginManager.Logger.Error("SetDead");
                PluginManager.Logger.Error(ex.Message);
                return false;
            }
        }

        // Inventory
        public async Task<Dictionary<string, InventoryItem>> GetInventoryItems()
        {
            Inventory = await GetDatabaseInventory();
            Dictionary<string, int> items = JsonConvert.DeserializeObject<Dictionary<string, int>>(Inventory);
            Dictionary<string, InventoryItem> invItems = new();

            foreach (KeyValuePair<string, int> item in items)
            {
                InventoryItem dbItem = await InventoryItem.GetItem(item.Key);
                if (item.Value == 0) continue; // if they don't have the item, no point of it being returned
                dbItem.Count = item.Value;
                invItems.Add(item.Key, dbItem);
            }

            return invItems;
        }

        internal async Task<bool> UpdateInventory()
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("characterId", CharacterId);
                dynamicParameters.Add("inventory", Inventory);
                bool result = await DapperDatabase<bool>.ExecuteAsync($"UPDATE characters SET `inventory` = @inventory WHERE `charIdentifier` = @characterId;", dynamicParameters);
                await Common.MoveToMainThread();
                return result;
            }
            catch (Exception ex)
            {
                PluginManager.Logger.Error("UpdateInventory");
                PluginManager.Logger.Error(ex.Message);
                return false;
            }
        }

        internal async Task<string> GetDatabaseInventory()
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("characterId", CharacterId);
                Inventory = await DapperDatabase<string>.GetSingleAsync($"select `inventory` from characters WHERE `charIdentifier` = @characterId;", dynamicParameters);

                return Inventory;
            }
            catch (Exception ex)
            {
                PluginManager.Logger.Error("UpdateInventory");
                PluginManager.Logger.Error(ex.Message);
                return "{}";
            }
        }

        // loadout
        internal async Task<List<Loadout>> GetDatabaseLoadout()
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("characterId", CharacterId);
                List<Loadout> loadouts = await DapperDatabase<Loadout>.GetListAsync($"select * from loadout WHERE `charIdentifier` = @characterId;", dynamicParameters);
                await Common.MoveToMainThread();
                return loadouts;
            }
            catch (Exception ex)
            {
                PluginManager.Logger.Error("UpdateInventory");
                PluginManager.Logger.Error(ex.Message);
                return new();
            }
        }

        // Save
        internal async Task<bool> Save()
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                // I hate that this is in this table, should be a UserID. SQL Refactor required
                // TODO: Refactor SQL so users have a unique key that is NOT the Steam ID
                // (framework should work without a steam requirement, this is why CFX keeps having issues, bloody steam!!!)
                dynamicParameters.Add("identifier", SteamIdentifier);
                if (CharacterId > 0) dynamicParameters.Add("characterId", CharacterId);
                dynamicParameters.Add("group", Group);
                dynamicParameters.Add("money", Cash);
                dynamicParameters.Add("gold", Gold);
                dynamicParameters.Add("rol", RoleToken);
                dynamicParameters.Add("xp", Experience);
                dynamicParameters.Add("inventory", Inventory);
                dynamicParameters.Add("job", Job);
                dynamicParameters.Add("status", Status);
                dynamicParameters.Add("firstname", Firstname);
                dynamicParameters.Add("lastname", Lastname);
                dynamicParameters.Add("skinPlayer", Skin);
                dynamicParameters.Add("compPlayer", Components);
                dynamicParameters.Add("jobgrade", JobGrade);
                dynamicParameters.Add("coords", Coords);
                dynamicParameters.Add("dead", IsDead);

                // Need two queries...first one will add a new character
                string query = @"INSERT INTO characters
                    (`identifier`,`group`,`money`,`gold`,`rol`,`xp`,`inventory`,`job`,
                    `status`,`firstname`,`lastname`,`skinPlayer`,`compPlayer`,`jobgrade`,
                    `coords`,`isdead`)
                    VALUES (@identifier, @group, @money, @gold, @rol, @xp, @inventory, @job, @status,
                            @firstname, @lastname, @skinPlayer, @compPlayer, @jobGrade, @coords, @dead);";

                // if its an existing character, we just need to update
                if (CharacterId > 0)
                    query = @"update characters set
                    `group` = @group,`money` = @money,`gold` = @gold,
                    `rol` = @rol,`xp` = @xp,`inventory` = @inventory,`job` = @job,
                    `status` = @status,`firstname` = @firstname,`lastname` = @lastname,
                    `skinPlayer` = @skinPlayer,`compPlayer` = @compPlayer,`jobgrade` = @jobGrade,
                    `coords` = @coords,`isdead` = @dead
                    WHERE
                        `charIdentifier` = @characterId;";

                return await DapperDatabase<bool>.ExecuteAsync(query, dynamicParameters);
            }
            catch (Exception ex)
            {
                PluginManager.Logger.Error("Save Character");
                PluginManager.Logger.Error(ex.Message);
                return false;
            }
        }

        internal async Task<bool> Delete()
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("characterId", CharacterId);
                return await DapperDatabase<bool>.ExecuteAsync($"DELETE FROM characters WHERE `charIdentifier` = @characterId;", dynamicParameters);
            }
            catch (Exception ex)
            {
                PluginManager.Logger.Error("Delete Character");
                PluginManager.Logger.Error(ex.Message);
                return false;
            }
        }

        internal async void SetExperienceAsync(int experience)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("characterId", CharacterId);
            dynamicParameters.Add("xp", experience);
            int result = await DapperDatabase<int>.GetSingleAsync($"UPDATE characters SET `xp` = @xp WHERE `charIdentifier` = @characterId; select `xp` from characters where `charIdentifier` = @characterId;", dynamicParameters);
            await Common.MoveToMainThread();
            Experience = result;
        }

        internal async void SetCashAsync(double amount)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("characterId", CharacterId);
            dynamicParameters.Add("money", amount);
            int result = await DapperDatabase<int>.GetSingleAsync($"UPDATE characters SET `money` = @money WHERE `charIdentifier` = @characterId; select `money` from characters where `charIdentifier` = @characterId;", dynamicParameters);
            await Common.MoveToMainThread();
            Cash = result;
        }

        internal async void SetGoldAsync(double amount)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("characterId", CharacterId);
            dynamicParameters.Add("gold", amount);
            int result = await DapperDatabase<int>.GetSingleAsync($"UPDATE characters SET `gold` = @gold WHERE `charIdentifier` = @characterId; select `gold` from characters where `charIdentifier` = @characterId;", dynamicParameters);
            await Common.MoveToMainThread();
            Gold = result;
        }

        internal async void SetRoleTokenAsync(double amount)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("characterId", CharacterId);
            dynamicParameters.Add("rol", amount);
            int result = await DapperDatabase<int>.GetSingleAsync($"UPDATE characters SET `rol` = @rol WHERE `charIdentifier` = @characterId; select `rol` from characters where `charIdentifier` = @characterId;", dynamicParameters);
            await Common.MoveToMainThread();
            RoleToken = result;
        }

        internal async void SetFirstnameAsync(string firstname)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("characterId", CharacterId);
            dynamicParameters.Add("firstname", firstname);
            string result = await DapperDatabase<string>.GetSingleAsync($"UPDATE characters SET `firstname` = @firstname WHERE `charIdentifier` = @characterId; select `firstname` from characters where `charIdentifier` = @characterId;", dynamicParameters);
            await Common.MoveToMainThread();
            Firstname = result;
        }

        internal async void SetLastnameAsync(string lastname)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("characterId", CharacterId);
            dynamicParameters.Add("lastname", lastname);
            string result = await DapperDatabase<string>.GetSingleAsync($"UPDATE characters SET `lastname` = @lastname WHERE `charIdentifier` = @characterId; select `lastname` from characters where `charIdentifier` = @characterId;", dynamicParameters);
            await Common.MoveToMainThread();
            Lastname = result;
        }

#endif

        #endregion
    }
}
