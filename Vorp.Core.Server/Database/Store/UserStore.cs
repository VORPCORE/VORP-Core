using Dapper;
using System.Threading.Tasks;
using Vorp.Core.Server.Models;
using Vorp.Shared.Records;

namespace Vorp.Core.Server.Database.Store
{
    internal static class UserStore
    {
        static ServerConfig _srvCfg => ServerConfiguration.Config();

        public static async Task<int> GetCountOfUsers()
        {
            return await DapperDatabase<int>.GetSingleAsync("SELECT COUNT(*) FROM users;");
        }

        public static async Task<bool> IsUserInWhitelist(string steamIdent)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("steam", steamIdent);
            return await DapperDatabase<bool>.GetSingleAsync("SELECT TRUE FROM whitelist WHERE `identifier` = @steam LIMIT 1;", dynamicParameters);
        }

        // still use this, though txAdmin does it any way
        public static async Task<bool> IsUserBanned(string steamIdent)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("steam", steamIdent);
            return await DapperDatabase<bool>.GetSingleAsync("SELECT TRUE FROM users WHERE `identifier` = @steam and `banned` = 1 LIMIT 1;", dynamicParameters);
        }

        internal static async Task<User> GetUser(string serverId, string steamIdent, string license, bool withCharacters = false)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("steam", steamIdent);
            User user = await DapperDatabase<User>.GetSingleAsync("SELECT * FROM users WHERE `identifier` = @steam LIMIT 1;", dynamicParameters);
            if (user == null)
            {
                user = new User(serverId, steamIdent, license, _srvCfg.UserConfig.NewUserGroup, 0);

                // if they are the first user, then set them as an admin
                bool isFirstUser = await GetCountOfUsers() == 0;
                if (isFirstUser)
                    await user.SetGroup("admin", true);

                bool saved = await user.Save();

                if (!saved)
                    return null;
            }

            if (withCharacters)
                await user.GetCharacters(); // Assign characters here

            return user;
        }
    }
}
