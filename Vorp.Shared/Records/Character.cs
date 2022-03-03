#if SERVER
using Dapper;
using Vorp.Core.Server.Database;
using System.Threading.Tasks;
#endif

using Lusive.Events.Attributes;
using System.ComponentModel;
using System.IO;
using Vorp.Shared.Models;
using System.Collections.Generic;

namespace Vorp.Shared.Records
{
    [Serialization]
    public partial class Character
    {
        public Character(int charidentifier, string firstname, string lastname)
        {
            CharacterId = charidentifier;
            Firstname = firstname;
            Lastname = lastname;
        }

        // I DON'T KNOW WHY, I DON'T WANT TO KNOW WHY, BUT, IF YOU CAN FIX IT, MAY THE GENERATOR SEE IN YOUR FAVOUR!
#if CLIENT
        public Character(BinaryReader binaryReader)
        {
            SteamIdentifier = binaryReader.ReadString();
            SteamName = binaryReader.ReadString();
            CharacterId = binaryReader.ReadInt32();
            Group = binaryReader.ReadString();
            Cash = binaryReader.ReadDouble();
            Gold = binaryReader.ReadDouble();
            RoleToken = binaryReader.ReadDouble();
            Experience = binaryReader.ReadInt32();
            Inventory = binaryReader.ReadString();
            Job = binaryReader.ReadString();
            Status = binaryReader.ReadString();
            Meta = binaryReader.ReadString();
            Firstname = binaryReader.ReadString();
            Lastname = binaryReader.ReadString();
            Skin = binaryReader.ReadString();
            Components = binaryReader.ReadString();
            JobGrade = binaryReader.ReadInt32();
            Coords = binaryReader.ReadString();
            IsDead = binaryReader.ReadBoolean();
            ClanId = binaryReader.ReadInt32();
            Trust = binaryReader.ReadInt32();
            Supporter = binaryReader.ReadInt32();
            Walk = binaryReader.ReadString();
            Crafting = binaryReader.ReadString();
            Info = binaryReader.ReadString();
            GunSmith = binaryReader.ReadDouble();
        }
#elif SERVER
        public Character(BinaryReader binaryReader, bool buildMe = false)
        {
            SteamIdentifier = binaryReader.ReadString();
            SteamName = binaryReader.ReadString();
            CharacterId = binaryReader.ReadInt32();
            Group = binaryReader.ReadString();
            Cash = binaryReader.ReadDouble();
            Gold = binaryReader.ReadDouble();
            RoleToken = binaryReader.ReadDouble();
            Experience = binaryReader.ReadInt32();
            Inventory = binaryReader.ReadString();
            Job = binaryReader.ReadString();
            Status = binaryReader.ReadString();
            Meta = binaryReader.ReadString();
            Firstname = binaryReader.ReadString();
            Lastname = binaryReader.ReadString();
            Skin = binaryReader.ReadString();
            Components = binaryReader.ReadString();
            JobGrade = binaryReader.ReadInt32();
            Coords = binaryReader.ReadString();
            IsDead = binaryReader.ReadBoolean();
            ClanId = binaryReader.ReadInt32();
            Trust = binaryReader.ReadInt32();
            Supporter = binaryReader.ReadInt32();
            Walk = binaryReader.ReadString();
            Crafting = binaryReader.ReadString();
            Info = binaryReader.ReadString();
            GunSmith = binaryReader.ReadDouble();
        }
#endif

        #region Fields
        [Description("identifier")]
        public string SteamIdentifier { get; set; } = default!;
        [Description("steamname")]
        public string SteamName { get; set; } = default!;
        [Description("charidentifier")]
        public int CharacterId { get; private set; }
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
        public string Firstname { get; private set; }
        [Description("lastname")]
        public string Lastname { get; private set; }
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
                        Logger.Error($"Character.AdjustCurrency {currency} value is unknown");
                        return false;
                }
                await BaseScript.Delay(0);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "AdjustCurrency");
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
                await BaseScript.Delay(0);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "AdjustExperience");
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
                await BaseScript.Delay(0);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SetJob");
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
                await BaseScript.Delay(0);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SetJobGrade");
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
                await BaseScript.Delay(0);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SetJobAndGrade");
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
                await BaseScript.Delay(0);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SetGroup");
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
                await BaseScript.Delay(0);
                return result > 0; // if result is true, then user is dead
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SetDead");
                return false;
            }
        }

        // Inventory
        public async Task<Dictionary<string, InventoryItem>> GetInventoryItems()
        {
            Inventory = await GetDatabaseInventory();
            Dictionary<string, int> items = JsonConvert.DeserializeObject<Dictionary<string, int>>(Inventory);
            Dictionary<string, InventoryItem> invItems = new();

            foreach(KeyValuePair<string, int> item in items)
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
                await BaseScript.Delay(0);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "UpdateInventory");
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
                Logger.Error(ex, "UpdateInventory");
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
                await BaseScript.Delay(0);
                return loadouts;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "UpdateInventory");
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
                    `identifier` = @identifier,`group` = @group,`money` = @money,`gold` = @gold,
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
                Logger.Error(ex, "Save Character");
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
                Logger.Error(ex, "Delete Character");
                return false;
            }
        }

        internal void SetExperience(int experience)
        {
            Experience = experience;
        }

        internal void SetCash(double amount)
        {
            Cash = amount;
        }

        internal void SetGold(double amount)
        {
            Gold = amount;
        }

        internal void SetRoleToken(double amount)
        {
            RoleToken = amount;
        }

#endif

        #endregion
    }
}
