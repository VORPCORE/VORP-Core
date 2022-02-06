using System.Collections.Generic;
using System.Threading.Tasks;
using Vorp.Core.Server.Commands;
using Vorp.Core.Server.Database.Store;
using Vorp.Core.Server.Web;
using Vorp.Shared.Data;
using Vorp.Shared.Models;
using Vorp.Shared.Records;

namespace Vorp.Core.Server.Managers
{
    public class UserManager : Manager<UserManager>
    {
        ServerConfigManager _srvCfg => ServerConfigManager.GetModule();
        DiscordClient _discord => DiscordClient.GetModule();

        long lastTimeCleanupRan = 0;
        const int TWO_MINUTES = (1000 * 60) * 2;

        public override void Begin()
        {
            EventRegistry.Add("playerConnecting", new Action<Player, string, CallbackDelegate, dynamic>(OnPlayerConnecting));
            EventRegistry.Add("playerJoined", new Action<Player>(OnPlayerJoined));
            EventRegistry.Add("playerDropped", new Action<Player, string>(OnPlayerDropped));
            EventRegistry.Add("onResourceStop", new Action<string>(OnResourceStop));
            lastTimeCleanupRan = GetGameTimer();
        }

        private async void OnResourceStop(string resourceName)
        {
            if (resourceName != GetCurrentResourceName()) return;
            foreach(KeyValuePair<string, User> kvp in UserSessions)
            {
                try
                {
                    await kvp.Value.ActiveCharacter.Save();
                }
                catch
                {
                    // no point reporting the error, just try the next user before the server shuts down
                    continue;
                }
            }
        }

        private void OnPlayerJoined([FromSource] Player player)
        {
            if (!UserSessions.ContainsKey(player.Handle)) return;
            User user = UserSessions[player.Handle];
            user.IsActive = true;
            if (Instance.IsOneSyncEnabled)
            {
                player.State.Set(StateBagKey.PlayerName, player.Name, true);
            }

            // add suggestions
            foreach(KeyValuePair<CommandContext, List<Tuple<CommandInfo, ICommand>>> entry in Instance.CommandFramework.Registry)
            {
                CommandContext commandContext = entry.Key;
                List<Tuple<CommandInfo, ICommand>> tuples = entry.Value;
                
                if (commandContext.IsRestricted && commandContext.RequiredRoles.Contains(user.Group))
                {
                    foreach (Tuple<CommandInfo, ICommand> item in tuples)
                    {
                        player.TriggerEvent("chat:addSuggestion", $"/{commandContext.Aliases[0]} {item.Item1.Aliases[0]}", $"{item.Item1.Description}");
                    }
                }
            }
        }

        private async void OnPlayerDropped([FromSource] Player player, string reason)
        {
            Logger.Info($"Player '{player.Name}' dropped (Reason: {reason}).");
            if (!UserSessions.ContainsKey(player.Handle)) return;
            User user = UserSessions[player.Handle];
            await user.ActiveCharacter.Save(); // save the characters information now, just to be sure
            // We do not remove the player straight away as other resources may request data when a player drops
            user.MarkPlayerHasDropped();
        }

        [TickHandler]
        private async Task OnPlayerCleanUp()
        {
            if ((GetGameTimer() - lastTimeCleanupRan) > TWO_MINUTES)
            {
                // copy the active user list so we don't run into any errors
                Dictionary<string, User> users = new Dictionary<string, User>(UserSessions);

                // loop each user in the active list
                foreach(KeyValuePair<string, User> kvp in users)
                {
                    User user = kvp.Value;
                    if (user.ActiveCharacter != null)
                    {
                        Player player = PlayersList[user.ServerId];
                        if (player != null && Instance.IsOneSyncEnabled)
                        {
                            Vector3 playerPosition = player.Character.Position;
                            float playerHeading = player.Character.Heading;
                            JsonBuilder jb = new();
                            jb.Add("x", playerPosition.X);
                            jb.Add("y", playerPosition.Y);
                            jb.Add("z", playerPosition.Z);
                            jb.Add("heading", playerHeading);
                            user.ActiveCharacter.Coords = $"{jb}";

                            UserSessions[kvp.Key].ActiveCharacter.Coords = user.ActiveCharacter.Coords;
                        }
                        await user.ActiveCharacter.Save();
                        await user.Save();
                    }
                    
                        // if its been over two minutes since we last saw them, remove them
                    if ((GetGameTimer() - user.GameTimeWhenDropped) > TWO_MINUTES)
                    {
                        await user.ActiveCharacter.Save();
                        bool isEndpointClear = string.IsNullOrEmpty(user.Endpoint);
                        if (isEndpointClear)
                            UserSessions.TryRemove(kvp.Key, out User removedUser);
                    }
                }

                lastTimeCleanupRan = GetGameTimer();
            }
            await BaseScript.Delay(5000); // run every 5 seconds
        }

        private async void OnPlayerConnecting([FromSource] Player player, string name, CallbackDelegate denyWithReason, dynamic deferrals)
        {
            deferrals.update(_srvCfg.GetTranslation("CheckingIdentifier"));

            string steamId = player?.Identifiers["steam"] ?? string.Empty; // maybe...

            if (string.IsNullOrEmpty(steamId))
            {
                DefferAndKick("error_steam_not_found", denyWithReason, deferrals);
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
                DefferAndKick("user_is_banned", denyWithReason, deferrals);
                return;
            }

            // Database whitelisting, legacy support
            if (_srvCfg.IsWhitelistDatabase)
            {
                bool isUserInWhitelist = await UserStore.IsUserInWhitelist(steamDatabaseIdentifier);
                if (isUserInWhitelist) goto USER_CAN_ENTER; // skip over, goto is old, but handy to jump around

                DefferAndKick("user_is_not_whitelisted", denyWithReason, deferrals);
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

                DefferAndKick("user_is_not_whitelisted", denyWithReason, deferrals);
                return;
            }

        USER_CAN_ENTER:
            deferrals.update(_srvCfg.GetTranslation("user_is_loading"));
            bool isCurrentlyConnected = Instance.IsUserActive(steamDatabaseIdentifier);
            if (isCurrentlyConnected)
            {
                // need to check some extras, so that if the SteamID matches a live player
                // if some other information differs, it should drop them
                User user = UserSessions[player.Handle];

                if (user.LicenseIdentifier != license)
                {
                    DefferAndKick("error_user_with_matching_steam_already_connected", denyWithReason, deferrals);
                    return;
                }
                // should this fire an event at the player?! It honestly should, then they know to request a character list
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

                UserSessions.TryAdd(player.Handle, user);
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
