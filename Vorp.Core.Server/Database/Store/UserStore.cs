using Dapper;
using System.Threading.Tasks;

namespace Vorp.Core.Server.Database.Store
{
    internal static class UserStore
    {
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
    }
}
