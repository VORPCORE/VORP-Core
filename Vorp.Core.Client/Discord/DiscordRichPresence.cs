using System;

namespace Vorp.Core.Client.Discord
{
    internal class DiscordRichPresence
    {
        public string AppId { get; set; }
        public string Asset { get; set; }
        public string AssetText { get; set; }

        public string SmallAsset { get; set; }
        public string SmallAssetText { get; set; }
        public string Status { get; set; }

        public DiscordRichPresence(string appId, string asset, string assetText)
        {
            AppId = appId;
            Asset = asset;
            AssetText = assetText;

            // Commented out until the exist, natives list says they should be they do not
            // string serverAddress = API.GetCurrentServerEndpoint();
            // API.SetDiscordRichPresenceAction(0, "Join Server", $"fivem://connect/{serverAddress}");
            // API.SetDiscordRichPresenceAction(1, "Visit Forums", "https://forums.example.net");
        }

        public void Commit()
        {
            if (string.IsNullOrEmpty(AppId))
            {
                throw new ArgumentNullException("AppId must be supplied");
            }

            API.SetDiscordAppId(AppId);
            API.SetDiscordRichPresenceAssetSmall(Asset);
            API.SetDiscordRichPresenceAssetText(AssetText);

            if (SmallAsset != null && SmallAssetText != null)
            {
                API.SetDiscordRichPresenceAssetSmall(SmallAsset);
                API.SetDiscordRichPresenceAssetSmallText(SmallAssetText);
            }

            if (Status != null) API.SetRichPresence($"{Status}");
        }
    }
}
