using System.Collections.Generic;
using Vorp.Shared.Records;

namespace Vorp.Core.Server.Commands.Impl
{
    public class StaffCommands : CommandContext
    {
        public override string[] Aliases { get; set; } = { "admin", "a", "staff" };
        public override string Title { get; set; } = "Staff Server Commands";
        public override bool IsRestricted { get; set; } = true;
        public override List<string> RequiredRoles { get; set; } = new List<string>() { "admin" };

        [CommandInfo(new[] { "helloWorld" })]
        public class HelloWorld : ICommand
        {
            public void On(User user, Player player, List<string> arguments)
            {
                Logger.Debug($"Hello World");
            }
        }
    }
}
