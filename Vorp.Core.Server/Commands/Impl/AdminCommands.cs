using System.Collections.Generic;
using Vorp.Shared.Records;

namespace Vorp.Core.Server.Commands.Impl
{
    public class AdminCommands : CommandContext
    {
        public override string[] Aliases { get; set; } = { "admin" };
        public override string Title { get; set; } = "Staff Server Commands";
        public override bool IsRestricted { get; set; } = true;
        public override List<string> RequiredRoles { get; set; } = new List<string>() { "admin" };

        [CommandInfo(new[] { "helloWorld", "hw" }, "Server will print Hello World in Console.")]
        public class HelloWorld : ICommand
        {
            public void On(User user, Player player, List<string> arguments)
            {
                Logger.Trace($"Hello World");
            }
        }

        [CommandInfo(new[] { "group" }, "This will allow setting of a players group. /admin group <playerId> <user/char> <groupName>")]
        public class GroupCommand : ICommand
        {
            public async void On(User user, Player player, List<string> arguments)
            {
                if (arguments.Count < 3) return;

                int targetId = 0;
                if (!int.TryParse(arguments[0], out targetId)) return;

                User targetUser = PluginManager.ToUser(targetId);
                if (targetUser is null) return;

                string type = arguments[1];
                bool result = false;

                switch (type)
                {
                    case "user":
                        result = await targetUser.SetGroup(arguments[2]);
                        break;
                    case "char":
                        result = await targetUser.ActiveCharacter.SetGroup(arguments[2]);
                        break;
                    default:
                        // Need notification of type unknown
                        break;
                }
            }
        }

        [CommandInfo(new[] { "job" }, "This will allow setting of a players job. /admin job <playerId> <jobName> <jobGrade?>")]
        public class JobCommand : ICommand
        {
            public async void On(User user, Player player, List<string> arguments)
            {
                int jobGrade = 0;
                if (arguments.Count < 2) return;

                if (arguments.Count == 3)
                {
                    if (!int.TryParse(arguments[2], out jobGrade)) return;
                }

                int targetId = 0;
                if (!int.TryParse(arguments[0], out targetId)) return;

                User targetUser = PluginManager.ToUser(targetId);
                if (targetUser is null) return;

                string job = arguments[1];
                bool result = false;

                if (arguments.Count == 3)
                    result = await targetUser.ActiveCharacter.SetJobAndGrade(job, jobGrade);

                if (arguments.Count == 2)
                    result = await targetUser.ActiveCharacter.SetJob(job);
            }
        }

        [CommandInfo(new[] { "setMoney" }, "This will allow setting players currency. /admin setMoney <playerId> <cash/gold/rol> <amount>")]
        public class SetMoneyCommand : ICommand
        {
            public void On(User user, Player player, List<string> arguments)
            {
                if (arguments.Count < 3) return;

                int targetId = 0;
                if (!int.TryParse(arguments[0], out targetId)) return;

                double amount = 0;
                if (!double.TryParse(arguments[2], out amount)) return;

                User targetUser = PluginManager.ToUser(targetId);
                if (targetUser is null) return;

                switch (arguments[1])
                {
                    case "cash":
                        targetUser.ActiveCharacter.SetCash(amount);
                        break;
                    case "gold":
                        targetUser.ActiveCharacter.SetGold(amount);
                        break;
                    case "rol":
                        targetUser.ActiveCharacter.SetRoleToken(amount);
                        break;
                    default:
                        // Need notification of type unknown
                        break;
                }
            }
        }

        [CommandInfo(new[] { "money" }, "This will allow adjusting players currency. /admin money <playerId> <cash/gold/rol> <add/rem> <amount>")]
        public class MoneyCommand : ICommand
        {
            public async void On(User user, Player player, List<string> arguments)
            {
                if (arguments.Count < 4) return;

                int targetId = 0;
                if (!int.TryParse(arguments[0], out targetId)) return;

                User targetUser = PluginManager.ToUser(targetId);
                if (targetUser is null) return;

                int currencyType = -1;
                switch (arguments[1])
                {
                    case "cash":
                        currencyType = 0;
                        break;
                    case "gold":
                        currencyType = 1;
                        break;
                    case "rol":
                        currencyType = 2;
                        break;
                    default:
                        // Need notification of type unknown
                        break;
                }

                bool increase = arguments[2] == "add";

                double amount = 0;
                if (!double.TryParse(arguments[3], out amount)) return;

                bool result = await targetUser.ActiveCharacter.AdjustCurrency(increase, currencyType, amount);
            }
        }

        [CommandInfo(new[] { "whitelist" }, "This helps mange the whitelist. /admin whitelist <add/rem> <steamId>")]
        public class WhitelistCommand : ICommand
        {
            public async void On(User user, Player player, List<string> arguments)
            {
                if (arguments.Count < 2) return;

                bool addToWhitelist = arguments[0] == "add";
                bool result = false;

                if (addToWhitelist)
                    result = await Database.Store.UserStore.AddUserToWhitelist(arguments[1]);

                if (!addToWhitelist)
                    result = await Database.Store.UserStore.RemoveUserFromWhitelist(arguments[1]);
            }
        }
    }
}
