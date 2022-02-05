namespace Vorp.Core.Server.Commands
{
    public class CommandInfo : Attribute
    {
        public string[] Aliases { get; set; }

        public CommandInfo(string[] aliases)
        {
            Aliases = aliases;
        }
    }
}