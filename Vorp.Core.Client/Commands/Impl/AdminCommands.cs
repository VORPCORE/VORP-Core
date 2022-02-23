using Vorp.Core.Client.Environment.Entities;

namespace Vorp.Core.Client.Commands.Impl
{
    public class AdminCommands : CommandContext
    {
        public override string[] Aliases { get; set; } = { "cAdmin" };
        public override string Title { get; set; } = "Staff Client Commands";
        public override bool IsRestricted { get; set; } = true;
        public override List<string> RequiredRoles { get; set; } = new List<string>() { "admin" };

        [CommandInfo(new[] { "ping" }, "Will respond with a pong.")]
        public class HelloWorld : ICommand
        {
            public void On(VorpPlayer player, List<string> arguments)
            {
                Logger.Trace($"Pong");
            }
        }
    }
}
