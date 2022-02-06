﻿using System.Threading.Tasks;
using Vorp.Core.Client.Managers;
using Vorp.Core.Client.RedM.Enums;

namespace Vorp.Core.Client.Environment.Entities
{
    class VorpPlayer
    {
        PluginManager pluginManager => PluginManager.Instance;
        ClientConfigManager configManager => ClientConfigManager.GetModule();

        public int PlayerId { get; private set; }
        public int ServerId { get; private set; }
        public int PlayerPedId { get; private set; }
        public string PlayerName { get; private set; }

        public int GamerTag { get; private set; }

        public VorpPlayer(int playerId, int playerPedId)
        {
            PlayerId = playerId;
            ServerId = GetPlayerServerId(playerId);
            PlayerPedId = playerPedId;
            PlayerName = GetPlayerName(playerId);

            pluginManager.AttachTickHandler(OnUpdate);
        }

        public void Dispose()
        {
            if (IsMpGamerTagActive(GamerTag))
            {
                VorpAPI.SetMpGamerTagVisibility(GamerTag, eUIGamertagVisibility.UIGAMERTAGVISIBILITY_NONE);
                API.RemoveMpGamerTag(GamerTag);
            }

            pluginManager.DetachTickHandler(OnUpdate);
        }

        private async Task OnUpdate()
        {
            if (configManager.PlayerNameConfig.Display)
            {
                string playerName = PlayerName;

                if (configManager.PlayerNameConfig.DisplayId)
                    playerName = $"{PlayerName} [{ServerId}]";

                GamerTag = VorpAPI.CreateGamerTag(PlayerId, playerName);
            }
            else
            {
                if (IsMpGamerTagActive(GamerTag))
                {
                    VorpAPI.SetMpGamerTagVisibility(GamerTag, eUIGamertagVisibility.UIGAMERTAGVISIBILITY_NONE);
                    API.RemoveMpGamerTag(GamerTag);
                }
            }
            await BaseScript.Delay(300);
        }
    }
}