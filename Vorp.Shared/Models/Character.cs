namespace Vorp.Shared.Models
{
    public record Character(int CharacterId, string Firstname, string Lastname)
    {
        public int CharacterId { get; private set; } = CharacterId;
        public string Firstname { get; private set; } = Firstname;
        public string Lastname { get; private set; } = Lastname;

        public bool IsActive { get; set; } = false;
        public double Cash { get; private set; } = 0;
        public double Gold { get; private set; } = 0;
        public double RoleToken { get; private set; } = 0;
    }
}
