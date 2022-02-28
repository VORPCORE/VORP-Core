﻿using Vorp.Core.Client.Environment.Entities;
using Vorp.Core.Client.Managers.Admin;

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

                    xStr = xStr.Replace(",", "").Replace("f", "");
                    yStr = yStr.Replace(",", "").Replace("f", "");
                    zStr = zStr.Replace(",", "").Replace("f", "");

                    float x = float.Parse(xStr);
                    float y = float.Parse(yStr);
                    float z = float.Parse(zStr);

                    var position = new Vector3(x, y, z);

                    await player.Teleport(position);
                }
                catch (Exception ex)
                {
                    // Chat.SendLocalMessage("Invalid or Missing Coord");
                }
            }
        }
    }
}