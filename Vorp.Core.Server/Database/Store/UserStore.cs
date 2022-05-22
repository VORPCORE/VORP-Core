using Dapper;
using System.Threading.Tasks;
using Vorp.Core.Server.Models;
using Vorp.Shared.Records;

namespace Vorp.Core.Server.Database.Store
{
    internal static class UserStore
    {
        static ServerConfig _srvCfg => ServerConfiguration.Config;

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

        public static async Task<bool> AddUserToWhitelist(string steamIdent)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("steam", steamIdent);
            return await DapperDatabase<bool>.ExecuteAsync("INSERT INTO whitelist (`identifier`) VALUES (@steam);", dynamicParameters);
        }

        public static async Task<bool> RemoveUserFromWhitelist(string steamIdent)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("steam", steamIdent);
            return await DapperDatabase<bool>.ExecuteAsync("DELETE FROM whitelist WHERE `identifier` = @steam;", dynamicParameters);
        }

        // still use this, though txAdmin does it any way
        public static async Task<bool> IsUserBanned(string steamIdent)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("steam", steamIdent);
            return await DapperDatabase<bool>.GetSingleAsync("SELECT TRUE FROM users WHERE `identifier` = @steam and `banned` = 1 LIMIT 1;", dynamicParameters);
        }

        internal static async Task<User> GetUser(string cfxServerHandle, string cfxName, string steamIdent, string license, bool withCharacters = false)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("steam", steamIdent);
            User user = await DapperDatabase<User>.GetSingleAsync("SELECT * FROM users WHERE `identifier` = @steam LIMIT 1;", dynamicParameters);

            await Common.MoveToMainThread();

            if (user is not null)
            {
                Logger.Debug($"User found with steamIdent [{user.SteamIdentifier}]");
                user.SetName(cfxName);
            }

            if (user == null)
            {
                Logger.Debug($"No user found with steamIdent [{steamIdent}]");
                user = new User(cfxServerHandle, cfxName, steamIdent, license, _srvCfg.UserConfig.NewUserGroup, 0);

                // if they are the first user, then set them as an admin
                int countOfUsers = await GetCountOfUsers();
                await Common.MoveToMainThread();
                bool isFirstUser = countOfUsers == 0;

                if (isFirstUser)
                    await user.SetGroup("admin", true);

                bool saved = await user.Save();
                await Common.MoveToMainThread();

                if (saved)
                    Logger.Debug($"Created a new user steamIdent [{user.SteamIdentifier}]");

                if (!saved)
                    return null;
            }

            if (withCharacters)
            {
                await BaseScript.Delay(1000);
                await user.GetCharacters(); // Assign characters here
                await Common.MoveToMainThread();
            }

            return user;
        }
    }
}
