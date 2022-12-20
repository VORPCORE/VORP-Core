using System.Collections.Generic;

namespace Vorp.Core.Server.Commands
{
    public class AdminCommands
    {
        [CommandAlias("Console prints 'Hello World'", "hw", "hello", "world", Restricted = false)]
        private void HelloWorld(int source, List<object> args, string rawCommand)
        {
            PluginManager.Logger.Debug($"Hello World");
        }

        [CommandAlias("Console prints 'pong'", "ping", Restricted = false)]
        private void Pong(int source, List<object> args, string rawCommand)
        {
            PluginManager.Logger.Debug($"Pong");
        }
    }
}
