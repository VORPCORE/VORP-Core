﻿namespace Vorp.Core.Client.Environment.Entities
{
    /*
     * TODO:
     * Add interactions: Revive, Heal, Carry
     * 
     */

    public class WorldPlayer : VorpPlayer
    {
        public int GamerTag { get; private set; }

        public override int PlayerPedId
        {
            get { return GetPlayerPed(PlayerId); }
        }

        public WorldPlayer(int playerId) : base(playerId)
        {
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
            if (clientConfig.PlayerNames.Display)
            {
                string playerName = PlayerName;

                if (clientConfig.PlayerNames.DisplayId)
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
