using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vorp.Core.Server.Managers;
using Vorp.Core.Server.Models;
using Vorp.Core.Server.Web.Discord.Entity;
using Vorp.Shared.Models;

namespace Vorp.Core.Server.Web
{
    public enum WebhookChannel
    {
        ServerPlayerLog,
        ServerErrorLog,
        ServerDebugLog,
    }

    public enum DiscordColor : int
    {
        White = 16777215,
        Black = 0,
        Red = 16738657,
        Green = 7855479,
        Blue = 11454159,
        Orange = 16757575
    }

    public class DiscordClient : Manager<DiscordClient>
    {
        static ServerConfig _srvCfg => ServerConfiguration.Config;
        static string _discordUrl => _srvCfg.Discord.Url;

        static Request request = new Request();
        public Dictionary<WebhookChannel, string> Webhooks = new Dictionary<WebhookChannel, string>();
        static long lastUpdate = GetGameTimer();
        static bool IsDelayRunnning = false;

        private static Regex _compiledUnicodeRegex = new Regex(@"[^\u0000-\u007F]", RegexOptions.Compiled);

        public override void Begin()
        {

        }

        public String StripUnicodeCharactersFromString(string inputValue)
        {
            return _compiledUnicodeRegex.Replace(inputValue, String.Empty);
        }

        [TickHandler]
        private async Task OnDiscordWebhookUpdate()
        {
            if ((GetGameTimer() - lastUpdate) > 120000)
            {
                lastUpdate = GetGameTimer();
                UpdateWebhooks();
            }

            while (Webhooks.Count == 0)
            {
                UpdateWebhooks();
                await BaseScript.Delay(1000);
                if (Webhooks.Count == 0)
                {
                    Logger.Error($"No Discord Webhooks returned, trying again in five seconds.");
                    await BaseScript.Delay(5000);
                }
            }

            await BaseScript.Delay(10000);
        }

        private async void UpdateWebhooks()
        {
            try
            {
                while (!Instance.IsServerReady)
                {
                    await BaseScript.Delay(1000);
                }

                Webhooks = new Dictionary<WebhookChannel, string>()
                {
                    { WebhookChannel.ServerPlayerLog, _srvCfg.Discord.Webhooks.ServerPlayerLog },
                    { WebhookChannel.ServerErrorLog, _srvCfg.Discord.Webhooks.ServerError },
                    { WebhookChannel.ServerDebugLog, _srvCfg.Discord.Webhooks.ServerDebug },
                };
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<RequestResponse> DiscordWebsocket(string method, string url, string jsonData = "")
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            headers.Add("Authorization", $"Bot {_srvCfg.Discord.BotKey}");
            return await request.HttpAsync($"{url}", method, jsonData, headers);
        }

        public async Task<RequestResponse> DiscordRequest(string method, string endpoint, string jsonData = "")
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            headers.Add("Authorization", $"Bot {_srvCfg.Discord.BotKey}");
            return await request.HttpAsync($"https://discordapp.com/api/{endpoint}", method, jsonData, headers);
        }

        public async Task<string> Avatar(ulong discordId)
        {
            string url = $"https://api.discord.wf/v2/users/{discordId}/avatar";
            RequestResponse requestResponse = await request.HttpAsync(url);
            if (requestResponse.status == System.Net.HttpStatusCode.OK)
            {
                DiscordAvatar avatar = JsonConvert.DeserializeObject<DiscordAvatar>(requestResponse.content);
                return avatar.Avatarurl;
            }
            return string.Empty;
        }

        // TODO: Add translation language keys for drop messages
        public async Task<bool> CheckDiscordIdIsInGuild(Player player)
        {
            bool IsMember = false;

            string discordIdStr = player.Identifiers["discord"];

            if (string.IsNullOrEmpty(discordIdStr))
            {
                Logger.Debug($"DiscordClient : {player.Name} not authorised with FiveM.");
                player.Drop($"Discord Identity failed validation, please restart FiveM and Discord. Make sure Discord is running on the same machine as FiveM. After you have opened Discord, then open FiveM, please check the #help-connecting channel for more information.\n\nDiscord URL: {_discordUrl}");
                return IsMember;
            }

            ulong discordId = 0;
            if (!ulong.TryParse(discordIdStr, out discordId))
            {
                Logger.Debug($"DiscordClient : {player.Name} Discord Information is invalid.");
                player.Drop($"Discord Identity failed validation, please restart FiveM and Discord. Make sure Discord is running on the same machine as FiveM. After you have opened Discord, then open FiveM, please check the #help-connecting channel for more information.\n\nDiscord URL: {_discordUrl}");
                return IsMember;
            }

            if (discordId == 0)
            {
                Logger.Debug($"DiscordClient : {player.Name} Discord ID is invalid, and not found.");
                player.Drop($"Discord Identity failed validation, please restart FiveM and Discord. Make sure Discord is running on the same machine as FiveM. After you have opened Discord, then open FiveM, please check the #help-connecting channel for more information.\n\nDiscord URL: {_discordUrl}");
                return IsMember;
            }

            RequestResponse requestResponse = await DiscordRequest("GET", $"guilds/{_srvCfg.Discord.GuildId}/members/{discordId}");
            await Common.MoveToMainThread();

            if (requestResponse.status == System.Net.HttpStatusCode.NotFound)
            {
                Logger.Debug($"DiscordClient : {player.Name} is NOT a member of the Discord Guild.");
                player.Drop($"This server requires you to be a member of their Discord and Verified (click the react role in the #verify-me channel), please check the #help-connecting channel, if you're still having issues please open a ticket.\n\nDiscord URL: {_discordUrl}");
                return IsMember;
            }

            if (!(requestResponse.status == System.Net.HttpStatusCode.OK))
            {
                Logger.Error($"DiscordClient : Error communicating with Discord");
                player.Drop($"Error communicating with Discord, please raise a support ticket or try connecting again later.");
                return IsMember;
            }

            DiscordMember discordMember = JsonConvert.DeserializeObject<DiscordMember>(requestResponse.content);

            ServerConfig serverConfig = ServerConfiguration.Config;

            if (serverConfig.Discord.Whitelist.Count == 0)
            {
                Logger.Error($"Whitelist is missing roles.");
                return true;
            }

            if (discordMember.Roles.Length == 0)
            {
                player.Drop($"This server does not have any roles configured on the discord server.");
                return false;
            }

            foreach(string role in discordMember.Roles)
            {
                foreach(string whitelistedRole in serverConfig.Discord.Whitelist)
                {
                    if (whitelistedRole == role)
                    {
                        IsMember = discordMember.JoinedAt.HasValue;
                        Logger.Trace($"DiscordClient : {player.Name} is a member of the Discord Guild.");
                        return true;
                    }
                }
            }

            player.Drop($"You do not have a whitelisted role on the server.");
            return false;
        }

        public async Task SendDiscordEmbededMessage(WebhookChannel webhookChannel, string name, string title, string description, DiscordColor discordColor)
        {
            try
            {
                if (!Webhooks.ContainsKey(webhookChannel))
                {
                    Logger.Warn($"SendDiscordEmbededMessage() -> Discord {webhookChannel} Webhook Missing");
                    return;
                }

                if (IsDelayRunnning) return;

                string cleanName = StripUnicodeCharactersFromString(name);

                string discordWebhook = Webhooks[webhookChannel];

                Webhook webhook = new Webhook(discordWebhook);

                webhook.Username = cleanName;

                Embed embed = new Embed();
                embed.Author = new EmbedAuthor { Name = cleanName };
                embed.Title = StripUnicodeCharactersFromString(title);
                embed.Description = StripUnicodeCharactersFromString(description);
                embed.Color = (int)discordColor;

                webhook.Embeds.Add(embed);
                await Common.MoveToMainThread();
                await webhook.Send();

                await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                Logger.Error($"SendDiscordEmbededMessage() -> {ex.Message}");
            }
        }

        public async Task SendDiscordSimpleMessage(WebhookChannel webhookChannel, string username, string name, string message)
        {
            try
            {
                string discordWebhook = Webhooks[webhookChannel];

                Webhook webhook = new Webhook(discordWebhook);

                webhook.Content = StripUnicodeCharactersFromString($"{name} > {message.Trim('"')}");
                webhook.Username = StripUnicodeCharactersFromString(username);

                await Common.MoveToMainThread();

                await webhook.Send();

                await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                Logger.Error($"SendDiscordSimpleMessage() -> {ex.Message}");
            }
        }
    }
}
