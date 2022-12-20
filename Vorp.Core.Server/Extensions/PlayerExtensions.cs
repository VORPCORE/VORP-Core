using Vorp.Shared.Records;

namespace Vorp.Core.Server.Extensions
{
    static class PlayerExtensions
    {
        public static int ServerHandle(this Player player) => int.Parse(player.Handle);

        public static User GetUser(this Player player)
        {
            return PluginManager.ToUser(player.Handle);
        }
    }
}
