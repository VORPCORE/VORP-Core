using Vorp.Core.Client.Environment.Entities;
using Vorp.Core.Client.Managers.Admin;
using Vorp.Shared.Commands;

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
                PluginManager.Logger.Info($"Pong");
            }
        }

        [CommandInfo(new[] { "scenario", "sce" }, "Will play a scenario, no params to stop.")]
        public class CmdScenario : ICommand
        {
            public void On(VorpPlayer player, List<string> arguments)
            {
                if (arguments.Count == 0)
                {
                    player.Character.ClearPedTasksImmediately();
                    return;
                }

                player.Character.TaskStartScenarioInPlace(arguments[0], player.Character.Heading);
            }
        }

        [CommandInfo(new[] { "time" }, "To test the time, only works on your client.")]
        public class ClientTime : ICommand
        {
            public void On(VorpPlayer player, List<string> arguments)
            {
                if (arguments.Count == 1)
                {
                    PluginManager.Instance.WorldTime.ClearClockTimeOverride();
                    return;
                }

                string strHour = arguments[0];
                string strMinute = arguments[1];

                if (int.TryParse(strHour, out int hour) && int.TryParse(strMinute, out int minute))
                {
                    PluginManager.Instance.WorldTime.ClockTimeOverride_2(hour, minute, pauseClock: true);
                }
            }
        }

        [CommandInfo(new[] { "noclip", "nc" }, "Will toggle noclip.")]
        public class NoClipToggle : ICommand
        {
            public void On(VorpPlayer player, List<string> arguments)
            {
                NoClipManager.GetModule().Toggle();
            }
        }

        [CommandInfo(new[] { "tp" }, "Will teleport you to the location provided. /cAdmin tp <X> <Y> <Z>")]
        public class Teleport : ICommand
        {
            public async void On(VorpPlayer player, List<string> arguments)
            {
                if (arguments.Count < 3) return;

                try
                {
                    string xStr = arguments[0];
                    string yStr = arguments[1];
                    string zStr = arguments[2];
                    string ground = "0";

                    if (arguments.Count == 4)
                        ground = arguments[3];

                    xStr = xStr.Replace(",", "").Replace("f", "");
                    yStr = yStr.Replace(",", "").Replace("f", "");
                    zStr = zStr.Replace(",", "").Replace("f", "");

                    float x = float.Parse(xStr);
                    float y = float.Parse(yStr);
                    float z = float.Parse(zStr);

                    var position = new Vector3(x, y, z);
                    bool findGround = ground == "1";

                    if (findGround)
                        await player.Character.Teleport(position, findGround);
                    else
                        player.Character.Position = position;
                }
                catch (Exception ex)
                {
                    // Chat.SendLocalMessage("Invalid or Missing Coord");
                }
            }
        }
    }
}
