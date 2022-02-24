using System.Collections.Generic;
using System.Threading.Tasks;
using Vorp.Core.Server.Commands;
using Vorp.Core.Server.Database.Store;
using Vorp.Core.Server.Events;
using Vorp.Core.Server.Web;
using Vorp.Shared.Data;
using Vorp.Shared.Models;
using Vorp.Shared.Records;

namespace Vorp.Core.Server.Managers
{
    public class UserManager : Manager<UserManager>
    {
        DiscordClient _discord => DiscordClient.GetModule();

        long lastTimeCleanupRan = 0;
        const int TWO_MINUTES = (1000 * 60) * 2;

        string DEFAULT_GROUP = ServerConfiguration.UserConfig.NewUserGroup;

        public override void Begin()
        {
            Event("playerConnecting", new Action<Player, string, CallbackDelegate, dynamic>(OnPlayerConnecting));
            Event("playerJoining", new Action<Player, string>(OnPlayerJoining));
            Event("playerDropped", new Action<Player, string>(OnPlayerDropped));
            Event("onResourceStop", new Action<string>(OnResourceStop));

            Event("vorp:user:activate", new Action<Player>(OnUserActivate));

            ServerGateway.Mount("vorp:user:active", new Func<ClientId, int, Task<string>>(OnUserActive));
            ServerGateway.Mount("vorp:user:list:active", new Func<ClientId, int, Task<List<dynamic>>>(OnGetActiveUserList));
            ServerGateway.Mount("vorp:user:group", new Func<ClientId, int, Task<string>>(OnGetUsersGroup));

            lastTimeCleanupRan = GetGameTimer();
        }

        private async Task<string> OnGetUsersGroup(ClientId source, int serverHandle)
        {
            Player player = PlayersList[source.Handle];
            if (player == null) return DEFAULT_GROUP;

            try
            {
                if (source.Handle != serverHandle) return DEFAULT_GROUP;
                if (source.User == null) return DEFAULT_GROUP;

                User user = source.User;
                return user.Group;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "OnGetUsersGroup");
                return DEFAULT_GROUP;
            }
        }

        private async void OnUserActivate([FromSource] Player player)
        {
            string steamId = player.Identifiers["steam"];
            string license = player.Identifiers["license"];

            if (UserSessions.ContainsKey(steamId)) return;

            User user = await UserStore.GetUser(player.Handle, player.Name, $"steam:{steamId}", license, true);
            UserSessions.TryAdd(steamId, user);

            user.IsActive = true;
            user.AddPlayer(player);
            user.UpdateServerId(player.Handle);

            SendPlayerChatSuggestions(player, user);
            SendPlayerCharacters(player, user);

            Logger.Trace($"{user.Group.ToUpper()} [{user.SteamIdentifier}] '{user.Player.Name}' is now Active!");
            ServerGateway.Send(player, "vorp:user:group:client", user.Group);
        }

        private async Task<string> OnUserActive(ClientId source, int serverHandle)
        {
            Player player = PlayersList[source.Handle];
            if (player == null) return "failed";

            try
            {
                if (source.Handle != serverHandle) return "failed";
                if (source.User == null) return "failed";

                User user = source.User;

                if (Instance.IsOneSyncEnabled)
                {
                    player.State.Set(StateBagKey.PlayerName, player.Name, true);
                }

                // add suggestions
                SendPlayerChatSuggestions(player, user);
                SendPlayerCharacters(player, user);

                Logger.Trace($"{user.Group.ToUpper()} [{user.SteamIdentifier}] '{user.Player.Name}' is now Active!");

                return user.Group;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "OnUserActive");
                return "failed";
            }
        }

        private void SendPlayerChatSuggestions(Player player, User user)
        {
            foreach (KeyValuePair<CommandContext, List<Tuple<CommandInfo, ICommand>>> entry in Instance.CommandFramework.Registry)
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
                else
                {
                    foreach (Tuple<CommandInfo, ICommand> item in tuples)
                    {
                        player.TriggerEvent("chat:addSuggestion", $"/{commandContext.Aliases[0]} {item.Item1.Aliases[0]}", $"{item.Item1.Description}");
                    }
                }
            }
        }

        void SendPlayerCharacters(Player player, User user)
        {
            // Legacy Methods to be removed
            // Make this internal to VORP_CORE
            int numberOfCharacters = user.NumberOfCharacters;
            Logger.Trace($"Player '{player.Name}' has {numberOfCharacters} Character(s) Loaded");

            ServerGateway.Send(player, "vorp:character:list", user.Characters, ServerConfiguration.MaximumCharacters);

            //if (numberOfCharacters <= 0)
            //{
            //    BaseScript.TriggerClientEvent(player, "vorpcharacter:createCharacter");
            //    Logger.Debug($"Player '{player.Name}' -> vorpcharacter:createCharacter");
            //}
            //else
            //{
            //    List<Dictionary<string, dynamic>> characters = new();

            //    foreach (KeyValuePair<int, Character> kvp in user.Characters)
            //    {
            //        Character character = kvp.Value;
            //        Dictionary<string, dynamic> characterDict = new Dictionary<string, dynamic>();
            //        characterDict.Add("charIdentifier", character.CharacterId);
            //        characterDict.Add("money", character.Cash);
            //        characterDict.Add("gold", character.Gold);
            //        characterDict.Add("firstname", character.Firstname);
            //        characterDict.Add("lastname", character.Lastname);
            //        characterDict.Add("skin", character.Skin);
            //        characterDict.Add("components", character.Components);
            //        characterDict.Add("coords", character.Coords);
            //        characterDict.Add("isDead", character.IsDead);
            //        characters.Add(characterDict);
            //    }

            //    if (ServerConfiguration.MaximumCharacters == 1 && numberOfCharacters <= 1)
            //    {
            //        BaseScript.TriggerClientEvent(player, "vorpcharacter:spawnUniqueCharacter");
            //        Logger.Debug($"Player '{player.Name}' -> vorpcharacter:spawnUniqueCharacter");
            //    }
            //    else
            //    {
            //        BaseScript.TriggerClientEvent(player, "vorpcharacter:selectCharacter", characters);
            //        Logger.Debug($"Player '{player.Name}' -> vorpcharacter:selectCharacter");
            //    }
            //}
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<List<dynamic>> OnGetActiveUserList(ClientId source, int id)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            List<dynamic> list = new List<dynamic>();
            if (source.Handle != id) return list;

            foreach (Player player in PlayersList)
            {
                list.Add(new { ServerId = player.Handle, Name = player.Name });
            }

            return list;
        }

        private async void OnResourceStop(string resourceName)
        {
            if (resourceName != GetCurrentResourceName()) return;
            foreach (KeyValuePair<string, User> kvp in UserSessions)
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

        private void OnPlayerJoining([FromSource] Player player, string oldId)
        {
            string steamId = player.Identifiers["steam"];

            if (!UserSessions.ContainsKey(steamId)) return;
            User user = UserSessions[steamId];
            user.IsActive = true;

            user.AddPlayer(player);
            user.UpdateServerId(player.Handle);

            Logger.Trace($"Player '{player.Name}' is joining.");
        }

        private async void OnPlayerDropped([FromSource] Player player, string reason)
        {
            Logger.Trace($"Player '{player.Name}' dropped (Reason: {reason}).");
            string steamId = player.Identifiers["steam"];
            if (!UserSessions.ContainsKey(steamId)) return;
            User user = UserSessions[steamId];

            if (user.ActiveCharacter != null)
                await user.ActiveCharacter.Save(); // save the characters information now, just to be sure

            // We do not remove the player straight away as other resources may request data when a player drops
            user.MarkPlayerHasDropped();
        }

        [TickHandler]
        private async Task OnPlayerCleanUp()
        {
            if ((GetGameTimer() - lastTimeCleanupRan) > TWO_MINUTES)
            {
                try
                {
                    // copy the active user list so we don't run into any errors
                    Dictionary<string, User> users = new Dictionary<string, User>(UserSessions);

                    // loop each user in the active list
                    foreach (KeyValuePair<string, User> kvp in users)
                    {
                        User user = kvp.Value;
                        if (user.ActiveCharacter is not null)
                        {
                            Player player = PlayersList[user.CFXServerID];
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

                        if (user.GameTimeWhenDropped > 0)
                        {
                            // if its been over two minutes since we last saw them, remove them
                            if ((GetGameTimer() - user.GameTimeWhenDropped) > TWO_MINUTES)
                            {
                                if (user.ActiveCharacter is not null)
                                    await user.ActiveCharacter.Save();

                                bool isEndpointClear = string.IsNullOrEmpty(user.Endpoint);
                                if (isEndpointClear)
                                    UserSessions.TryRemove(kvp.Key, out User removedUser);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"OnPlayerCleanUp");
                }

                lastTimeCleanupRan = GetGameTimer();
            }
            await BaseScript.Delay(5000); // run every 5 seconds
        }

        private async void OnPlayerConnecting([FromSource] Player player, string name, CallbackDelegate denyWithReason, dynamic deferrals)
        {
            deferrals.update(ServerConfiguration.GetTranslation("user_checking_identifier"));

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
            for (int i = 0; i < numberOfTokens; i++)
            {
                playerTokens.Add(GetPlayerToken(player.Handle, i));
            }

            string steamDatabaseIdentifier = $"steam:{steamId}"; // hate string keys, its so slow! maybe refactor in the future?

            // Check to see if the user is banned before we do anything else.
            bool isUserBanned = await UserStore.IsUserBanned(steamDatabaseIdentifier);
            await BaseScript.Delay(0);
            if (isUserBanned)
            {
                DefferAndKick("user_is_banned", denyWithReason, deferrals);
                return;
            }

            // Database whitelisting, legacy support
            if (ServerConfiguration.IsWhitelistDatabase)
            {
                bool isUserInWhitelist = await UserStore.IsUserInWhitelist(steamDatabaseIdentifier);
                await BaseScript.Delay(0);
                if (isUserInWhitelist) goto USER_CAN_ENTER; // skip over, goto is old, but handy to jump around

                DefferAndKick("user_is_not_whitelisted", denyWithReason, deferrals);
                return;
            }

            // Discord Role Whitelisting
            if (ServerConfiguration.IsWhitelistDiscord)
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
            deferrals.update(ServerConfiguration.GetTranslation("user_is_loading"));
            bool isCurrentlyConnected = Instance.IsUserActive(steamDatabaseIdentifier);
            if (isCurrentlyConnected)
            {
                // need to check some extras, so that if the SteamID matches a live player
                // if some other information differs, it should drop them
                User user = UserSessions[steamId];

                if (user.LicenseIdentifier == license)
                {
                    DefferAndKick("error_user_with_matching_steam_already_connected", denyWithReason, deferrals);
                    return;
                }

                // should this fire an event at the player?! It honestly should, then they know to request a character list
                Logger.Trace($"Player: [{player.Handle}] {player.Name} has re-joined the server.");
                deferrals.done();
            }

            if (!isCurrentlyConnected)
            {
                // either they are new, or already exist
                User user = await UserStore.GetUser(player.Handle, player.Name, steamDatabaseIdentifier, license, true);

                await BaseScript.Delay(0);

                if (user == null)
                {
                    DefferAndKick("error_creating_user", denyWithReason, deferrals);
                    deferrals.done();
                    return;
                }

                UserSessions.AddOrUpdate(steamId, user, (key, oldValue) => oldValue = user);

                Logger.Trace($"Player: [{steamId}] {player.Name} is connecting to the server with {user.NumberOfCharacters} character(s).");
                Logger.Trace($"Number of Sessions: {UserSessions.Count}");
                deferrals.done();
                return;

                // review if needed
                // DefferAndKick("error_creating_user_session", denyWithReason, deferrals);
            }
        }

        private void DefferAndKick(string languageKey, CallbackDelegate denyWithReason, dynamic deferrals)
        {
            string message = ServerConfiguration.GetTranslation(languageKey);
            deferrals.done(message);
            denyWithReason.Invoke(message);
        }
    }
}
