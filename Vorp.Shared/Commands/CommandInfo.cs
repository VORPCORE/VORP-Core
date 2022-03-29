namespace Vorp.Shared.Commands
{
    public class CommandInfo : Attribute
    {
        public string[] Aliases { get; set; }
        public string Description { get; set; }

        public CommandInfo(string[] aliases, string description)
        {
            Aliases = aliases;
            Description = description;
        }
    }
}
