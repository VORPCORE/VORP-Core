using System.Collections.Generic;
using Vorp.Core.Server.Database.Store;
using Vorp.Core.Server.Web;
using Vorp.Shared.Records;

namespace Vorp.Core.Server.Managers
{
    public class UserManager : Manager<UserManager>
    {
        ServerConfigManager _srvCfg => ServerConfigManager.GetModule();
        DiscordClient _discord => DiscordClient.GetModule();

        public override void Begin()
        {
            EventRegistry.Add("playerConnecting", new Action<Player, string, CallbackDelegate, dynamic>(OnPlayerConnecting));
        }

        private async void OnPlayerConnecting([FromSource] Player player, string name, CallbackDelegate denyWithReason, dynamic deferrals)
        {
            deferrals.update(_srvCfg.GetTranslation("CheckingIdentifier"));

            string steamId = player?.Identifiers["steam"] ?? string.Empty; // maybe...

            if (string.IsNullOrEmpty(steamId))
            {
                DefferAndKick("NoSteam", denyWithReason, deferrals);
                return;
            }

            string discordId = player?.Identifiers["discord"] ?? string.Empty;
            string license = player?.Identifiers["license2"] ?? string.Empty;

            // player tokens are hardware keys and other information, best to use for checking players when going over bans
            // Future feature
            // TODO: Finish ban feature
            List<string> playerTokens = new List<string>();
            int numberOfTokens = GetNumPlayerTokens(player.Handle);
            for(int i = 0; i < numberOfTokens; i++)
            {
                playerTokens.Add(GetPlayerToken(player.Handle, i));
            }

            string steamDatabaseIdentifier = $"steam:{steamId}"; // hate string keys, its so slow! maybe refactor in the future?

            // Check to see if the user is banned before we do anything else.
            bool isUserBanned = await UserStore.IsUserBanned(steamDatabaseIdentifier);
            if (isUserBanned)
            {
                DefferAndKick("BannedUser", denyWithReason, deferrals);
                return;
            }

            // Database whitelisting, legacy support
            if (_srvCfg.IsWhitelistDatabase)
            {
                bool isUserInWhitelist = await UserStore.IsUserInWhitelist(steamDatabaseIdentifier);
                if (isUserInWhitelist) goto USER_CAN_ENTER; // skip over, goto is old, but handy to jump around

                DefferAndKick("NoInWhitelist", denyWithReason, deferrals);
                return;
            }

            // Discord Role Whitelisting
            if (_srvCfg.IsWhitelistDiscord)
            {
                if (string.IsNullOrEmpty(discordId))
                {
                    DefferAndKick("error_discord_not_authorised", denyWithReason, deferrals);
                    return;
                }
                bool isUserRoleCorrect = await _discord.CheckDiscordIdIsInGuild(player);
                if (isUserRoleCorrect) goto USER_CAN_ENTER;

                DefferAndKick("error_discord_not_whitelisted", denyWithReason, deferrals);
                return;
            }

        USER_CAN_ENTER:
            deferrals.update(_srvCfg.GetTranslation("LoadingUser"));
            bool isCurrentlyConnected = Instance.IsUserActive(steamDatabaseIdentifier);
            if (isCurrentlyConnected)
            {
                // should this fire an event at the player?! It honestly should, then they know to request a character list
                User user = ActiveUsers[player.Handle];
                user.UpdateServerId(player.Handle); // update the serverId to be sure
                deferrals.done();
            }

            if (!isCurrentlyConnected)
            {
                // either they are new, or already exist
                User user = await UserStore.GetUser(player.Handle, steamId, license, true);

                if (user == null)
                {
                    DefferAndKick("error_creating_user", denyWithReason, deferrals);
                    return;
                }

                ActiveUsers.TryAdd(player.Handle, user);
                deferrals.done();
            }
        }

        private void DefferAndKick(string languageKey, CallbackDelegate denyWithReason, dynamic deferrals)
        {
            string message = _srvCfg.GetTranslation(languageKey);
            deferrals.done(message);
            denyWithReason.Invoke(message);
        }
    }
}
