using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vorp.Core.Server.Managers;
using Vorp.Core.Server.Web.Discord.Entity;
using Vorp.Shared.Models;

namespace Vorp.Core.Server.Web
{
    public enum WebhookChannel
    {
        Chat = 1,
        Report,
        StaffLog,
        ServerLog,
        ServerErrors,
        PlayerLog,
        ServerEventLog,
        PlayerDeathLog
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
        static ServerConfigManager _srvCfg => ServerConfigManager.GetModule();
        static string _discordUrl => _srvCfg.Discord.Url;

        static Request request = new Request();
        public Dictionary<WebhookChannel, DiscordWebhook> Webhooks = new Dictionary<WebhookChannel, DiscordWebhook>();
        static long lastUpdate = GetGameTimer();
        static string DATE_FORMAT = "yyyy-MM-dd HH:mm";
        static bool IsDelayRunnning = false;
        static long DelayMillis = 0;

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

        private void UpdateWebhooks()
        {
            Webhooks = new Dictionary<WebhookChannel, DiscordWebhook>()
            {

            };
        }

        public async Task<RequestResponse> DiscordWebsocket(string method, string url, string jsonData = "")
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            headers.Add("Authorization", $"Bot {_srvCfg.Discord.BotKey}");
            return await request.Http($"{url}", method, jsonData, headers);
        }

        public async Task<RequestResponse> DiscordRequest(string method, string endpoint, string jsonData = "")
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            headers.Add("Authorization", $"Bot {_srvCfg.Discord.BotKey}");
            return await request.Http($"https://discordapp.com/api/{endpoint}", method, jsonData, headers);
        }

        public async Task<string> Avatar(ulong discordId)
        {
            string url = $"https://api.discord.wf/v2/users/{discordId}/avatar";
            RequestResponse requestResponse = await request.Http(url);
            if (requestResponse.status == System.Net.HttpStatusCode.OK)
            {
                DiscordAvatar avatar = JsonConvert.DeserializeObject<DiscordAvatar>(requestResponse.content);
                return avatar.Avatarurl;
            }
            return string.Empty;
        }

        public async Task<bool> CheckDiscordIdIsInGuild(Player player)
        {
            bool IsMember = false;

            string discordIdStr = player.Identifiers["discord"];

            if (string.IsNullOrEmpty(discordIdStr))
            {
                Logger.Debug($"DiscordClient : {player.Name} not authorised with FiveM.");
                await BaseScript.Delay(0);
                player.Drop($"Discord Identity failed validation, please restart FiveM and Discord. Make sure Discord is running on the same machine as FiveM. After you have opened Discord, then open FiveM, please check the #help-connecting channel for more information.\n\nDiscord URL: {_discordUrl}");
                return IsMember;
            }

            ulong discordId = 0;
            if (!ulong.TryParse(discordIdStr, out discordId))
            {
                Logger.Debug($"DiscordClient : {player.Name} Discord Information is invalid.");
                await BaseScript.Delay(0);
                player.Drop($"Discord Identity failed validation, please restart FiveM and Discord. Make sure Discord is running on the same machine as FiveM. After you have opened Discord, then open FiveM, please check the #help-connecting channel for more information.\n\nDiscord URL: {_discordUrl}");
                return IsMember;
            }

            if (discordId == 0)
            {
                Logger.Debug($"DiscordClient : {player.Name} Discord ID is invalid, and not found.");
                await BaseScript.Delay(0);
                player.Drop($"Discord Identity failed validation, please restart FiveM and Discord. Make sure Discord is running on the same machine as FiveM. After you have opened Discord, then open FiveM, please check the #help-connecting channel for more information.\n\nDiscord URL: {_discordUrl}");
                return IsMember;
            }

            RequestResponse requestResponse = await DiscordRequest("GET", $"guilds/{_srvCfg.Discord.GuildId}/members/{discordId}");
            await BaseScript.Delay(0);

            if (requestResponse.status == System.Net.HttpStatusCode.NotFound)
            {
                Logger.Debug($"DiscordClient : {player.Name} is NOT a member of the Discord Guild.");
                await BaseScript.Delay(0);
                player.Drop($"This server requires you to be a member of their Discord and Verified (click the react role in the #verify-me channel), please check the #help-connecting channel, if you're still having issues please open a ticket.\n\nDiscord URL: {_discordUrl}");
                return IsMember;
            }

            if (!(requestResponse.status == System.Net.HttpStatusCode.OK))
            {
                Logger.Error($"DiscordClient : Error communicating with Discord");
                await BaseScript.Delay(0);
                player.Drop($"Error communicating with Discord, please raise a support ticket or try connecting again later.");
                return IsMember;
            }

            Member discordMember = JsonConvert.DeserializeObject<Member>(requestResponse.content);

            string verifiedRoleConvar = API.GetConvar("discord_verified_roleId", "ROLE_NOT_SET");

            if (verifiedRoleConvar != "ROLE_NOT_SET")
            {
                if (discordMember.Roles.Contains($"{verifiedRoleConvar}"))
                {
                    Logger.Debug($"DiscordClient : {player.Name} is a verified member of the Discord Guild.");
                    return true;
                }
                else
                {
                    Logger.Debug($"DiscordClient : {player.Name} is not a verified member of the Discord Guild.");
                    await BaseScript.Delay(0);
                    player.Drop($"This server requires you to be a member of their Discord and Verified.\n\nDiscord URL: {_discordUrl}");
                    return false;
                }
            }

            IsMember = discordMember.JoinedAt.HasValue;
            await BaseScript.Delay(0);
            Logger.Success($"DiscordClient : {player.Name} is a member of the Discord Guild.");

            return IsMember;
        }

        public async void SendChatMessage(string name, string message)
        {
            if (!Webhooks.ContainsKey(WebhookChannel.Chat))
            {
                Logger.Warn($"SendDiscordChatMessage() -> Discord Chat Webhook Missing");
                return;
            }

            await SendDiscordSimpleMessage(WebhookChannel.Chat, _srvCfg.Discord.Botname, name, message);
            await BaseScript.Delay(0);
        }

        public async void SendDiscordServerEventLogMessage(string message)
        {
            if (!Webhooks.ContainsKey(WebhookChannel.ServerEventLog))
            {
                Logger.Warn($"SendDiscordChatMessage() -> Discord Server Event Webhook Missing");
                return;
            }

            try
            {
                DiscordWebhook discordWebhook = Webhooks[WebhookChannel.ServerEventLog];

                Webhook webhook = new Webhook(discordWebhook.Url);

                webhook.AvatarUrl = discordWebhook.Avatar;
                webhook.Content = StripUnicodeCharactersFromString($"{message.Trim('"')}");
                webhook.Username = StripUnicodeCharactersFromString(_srvCfg.Discord.Botname);

                await BaseScript.Delay(0);
                await webhook.Send();

                await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                Logger.Error($"SendDiscordPlayerLogMessage() -> {ex.Message}");
            }

        }

        public async void SendDiscordPlayerDeathLogMessage(string message)
        {
            if (!Webhooks.ContainsKey(WebhookChannel.PlayerDeathLog))
            {
                Logger.Warn($"SendDiscordChatMessage() -> Discord Player Death Log Webhook Missing");
                return;
            }

            try
            {
                DiscordWebhook discordWebhook = Webhooks[WebhookChannel.PlayerDeathLog];

                Webhook webhook = new Webhook(discordWebhook.Url);

                webhook.AvatarUrl = discordWebhook.Avatar;
                webhook.Content = StripUnicodeCharactersFromString($"{message}");
                webhook.Username = StripUnicodeCharactersFromString(_srvCfg.Discord.Botname);

                await BaseScript.Delay(0);

                await webhook.Send();

                await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                Logger.Error($"SendDiscordPlayerDeathLogMessage() -> {ex.Message}");
            }

        }

        public async void SendDiscordPlayerLogMessage(string message)
        {
            if (!Webhooks.ContainsKey(WebhookChannel.PlayerLog))
            {
                Logger.Warn($"SendDiscordChatMessage() -> Discord Player Log Webhook Missing");
                return;
            }

            try
            {
                DiscordWebhook discordWebhook = Webhooks[WebhookChannel.PlayerLog];

                Webhook webhook = new Webhook(discordWebhook.Url);

                webhook.AvatarUrl = discordWebhook.Avatar;
                webhook.Content = StripUnicodeCharactersFromString($"{message}");
                webhook.Username = StripUnicodeCharactersFromString(_srvCfg.Discord.Botname);

                await BaseScript.Delay(0);

                await webhook.Send();

                await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                Logger.Error($"SendDiscordPlayerLogMessage() -> {ex.Message}");
            }

        }

        public async void SendDiscordStaffLogMessage(string adminName, string player, string action, string reason, string duration = "")
        {
            try
            {
                if (!Webhooks.ContainsKey(WebhookChannel.StaffLog))
                {
                    Logger.Warn($"SendDiscordStaffLogMessage() -> Discord {WebhookChannel.StaffLog} Webhook Missing");
                    return;
                }

                if (IsDelayRunnning) return;

                DiscordWebhook discordWebhook = Webhooks[WebhookChannel.StaffLog];

                Webhook webhook = new Webhook(discordWebhook.Url);

                webhook.AvatarUrl = discordWebhook.Avatar;
                webhook.Username = "Staff";

                Embed embed = new Embed();
                embed.Author = new EmbedAuthor { Name = adminName, IconUrl = discordWebhook.Avatar };
                embed.Title = StripUnicodeCharactersFromString($"Player: {player}");

                embed.Description = StripUnicodeCharactersFromString($" **{action}**: {reason}");
                if (!string.IsNullOrEmpty(duration))
                    embed.Description = StripUnicodeCharactersFromString($" **{action}**: {reason} \n **Duration**: {duration}");

                embed.Color = (int)DiscordColor.Orange;
                if (action == "Ban")
                    embed.Color = (int)DiscordColor.Red;

                webhook.Embeds.Add(embed);
                await BaseScript.Delay(0);
                await webhook.Send();

                await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                Logger.Error($"SendDiscordStaffMessage() -> {ex.Message}");
            }
        }

        public async void SendDiscordReportMessage(string reporterName, string playerBeingReported, string reason)
        {
            try
            {
                if (!Webhooks.ContainsKey(WebhookChannel.Report))
                {
                    Logger.Warn($"SendDiscordReportMessage() -> Discord {WebhookChannel.Report} Webhook Missing");
                    return;
                }

                if (IsDelayRunnning) return;

                DiscordWebhook discordWebhook = Webhooks[WebhookChannel.Report];

                Webhook webhook = new Webhook(discordWebhook.Url);

                webhook.AvatarUrl = discordWebhook.Avatar;
                string cleanName = StripUnicodeCharactersFromString(_srvCfg.Discord.Botname);
                webhook.Username = cleanName;

                Embed embed = new Embed();
                embed.Author = new EmbedAuthor { Name = $"Report By: {reporterName}", IconUrl = discordWebhook.Avatar };
                embed.Title = $"Report";
                embed.Description = $"Player: {playerBeingReported}\nReason: {reason}";
                embed.Color = (int)DiscordColor.Blue;
                embed.Thumbnail = new EmbedThumbnail { Url = discordWebhook.Avatar };

                webhook.Embeds.Add(embed);
                await BaseScript.Delay(0);
                await webhook.Send();
            }
            catch (Exception ex)
            {
                Logger.Error($"SendDiscordReportMessage() -> {ex.Message}");
            }
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

                DiscordWebhook discordWebhook = Webhooks[webhookChannel];

                Webhook webhook = new Webhook(discordWebhook.Url);

                webhook.AvatarUrl = discordWebhook.Avatar;
                webhook.Username = cleanName;

                Embed embed = new Embed();
                embed.Author = new EmbedAuthor { Name = cleanName, IconUrl = discordWebhook.Avatar };
                embed.Title = StripUnicodeCharactersFromString(title);
                embed.Description = StripUnicodeCharactersFromString(description);
                embed.Color = (int)discordColor;
                embed.Thumbnail = new EmbedThumbnail { Url = discordWebhook.Avatar };

                webhook.Embeds.Add(embed);
                await BaseScript.Delay(0);
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
                DiscordWebhook discordWebhook = Webhooks[webhookChannel];

                Webhook webhook = new Webhook(discordWebhook.Url);

                webhook.AvatarUrl = discordWebhook.Avatar;
                webhook.Content = StripUnicodeCharactersFromString($"{name} > {message.Trim('"')}");
                webhook.Username = StripUnicodeCharactersFromString(username);

                await BaseScript.Delay(0);

                await webhook.Send();

                await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                Logger.Error($"SendDiscordSimpleMessage() -> {ex.Message}");
            }
        }

        async void DelayTriggered(long delayMillis)
        {
            DelayMillis = delayMillis * 1000;
        }

        async Task OnDelayCooldownTask()
        {
            long gameTimer = API.GetGameTimer();
            IsDelayRunnning = true;

            while ((API.GetGameTimer() - gameTimer) < DelayMillis)
            {
                await BaseScript.Delay(1000);
            }

            PluginManager.Instance.DetachTickHandler(OnDelayCooldownTask);
            IsDelayRunnning = false;
        }
    }
}
