#if SERVER
using Dapper;
using Vorp.Core.Server.Database;
using System.Threading.Tasks;
#endif

using System.ComponentModel;

namespace Vorp.Shared.Records
{
    public record Character(int CharacterId, string Firstname, string Lastname)
    {
        #region Fields
        [Description("identifier")]
        public string SteamIdentifier { get; set; } = default!;

        [Description("steamname")]
        public string SteamName { get; set; } = default!;

        [Description("charidentifier")]
        public int CharacterId { get; private set; } = CharacterId;

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

        [Description("inventory")]
        public string Inventory { get; set; } = "{}";

        [Description("job")]
        public string Job { get; private set; } = "unemployed";

        [Description("status")]
        public string Status { get; set; } = "{}";

        [Description("meta")]
        public string Meta { get; set; } = "{}";

        [Description("firstname")]
        public string Firstname { get; private set; } = Firstname;

        [Description("lastname")]
        public string Lastname { get; private set; } = Lastname;

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
        // TODO: Move all SQL into procedures
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
                Job = await DapperDatabase<string>.GetSingleAsync($"UPDATE characters SET `job` += @job WHERE `charIdentifier` = @characterId; SELECT `job` from characters WHERE `charIdentifier` = @characterId;", dynamicParameters);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SetJob");
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
                Group = await DapperDatabase<string>.GetSingleAsync($"UPDATE characters SET `group` += @group WHERE `charIdentifier` = @characterId; SELECT `group` from characters WHERE `charIdentifier` = @characterId;", dynamicParameters);
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
                int result = await DapperDatabase<int>.GetSingleAsync($"UPDATE characters SET `isdead` += @dead WHERE `charIdentifier` = @characterId; SELECT `isdead` from characters WHERE `charIdentifier` = @characterId;", dynamicParameters);
                return result > 0; // if result is true, then user is dead
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SetGroup");
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
