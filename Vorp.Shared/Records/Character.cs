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
        public string Group { get; set; } = "user";

        [Description("money")]
        public double Cash { get; private set; } = 0.00;

        [Description("gold")]
        public double Gold { get; private set; } = 0.00;

        [Description("rol")]
        public double RoleToken { get; private set; } = 0.00;

        [Description("xp")]
        public int xp { get; set; } = 0;

        [Description("inventory")]
        public string Inventory { get; set; } = "{}";

        [Description("job")]
        public string Job { get; set; } = "unemployed";

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
    }
}
