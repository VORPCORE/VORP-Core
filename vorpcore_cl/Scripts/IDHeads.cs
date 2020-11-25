using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace vorpcore_cl.Scripts
{
    public class IDHeads : BaseScript
    {
        public static bool UseIDHeads = false;
        public static bool UseKeyMode = false;
        public static Dictionary<int, int> PlayerTags = new Dictionary<int, int>();
        public static uint keyShow = 0;

        private static bool showIds = true;

        public IDHeads()
        {
            Tick += SetPlayerIdOnHead;
            Tick += KeyIdOnHead;
        }

        [Tick]
        private async Task KeyIdOnHead()
        {
            if (!UseIDHeads || !UseKeyMode) { return; }

            if (IsControlPressed(0, keyShow))
            {
                showIds = true;
            }
            else
            {
                showIds = false;
            }
            await Delay(0);

        }

        [Tick]
        private async Task SetPlayerIdOnHead()
        {
            if (!UseIDHeads) { return; }

            for (int i = 0; i < 255; i++)
            {

                if (NetworkIsPlayerActive(i))
                {
                    if (GetPlayerPed(i) != PlayerPedId())
                    {

                        if (PlayerTags.ContainsKey(i))
                        {
                            if (Function.Call<bool>((Hash)0x6E1C31E14C7A5F97, PlayerTags[i]))
                            {
                                float distanceConfig = Utils.GetConfig.Config["HeadIdDistance"].ToObject<float>();

                                if (GetDistanceFromPlayer(i) < distanceConfig && showIds)
                                {
                                    // Feature 2.0 Voice Chat
                                    //if () //NetworkIsPlayerTalking
                                    //{
                                    //    Function.Call((Hash)0x84BD27DDF9575816, PlayerTags[i], 0x333FC632);
                                    //}
                                    //else
                                    //{ 
                                    //    Function.Call((Hash)0x84BD27DDF9575816, PlayerTags[i], 0x42C33427);
                                    //}

                                    Function.Call((Hash)0x93171DDDAB274EB8, PlayerTags[i], 2);

                                }
                                else
                                {
                                    Function.Call((Hash)0x93171DDDAB274EB8, PlayerTags[i], 0);
                                }
                            }
                            else
                            {

                                int tagId = Function.Call<int>((Hash)0xD877AF112AD2B41B, i, GetPlayerServerId(i).ToString(), false, false, "", 0);
                                PlayerTags[i] = tagId;
                            }
                        }
                        else
                        {
                            int tagId = Function.Call<int>((Hash)0xD877AF112AD2B41B, i, GetPlayerServerId(i).ToString(), false, false, "", 0);
                            PlayerTags.Add(i, tagId);
                        }
                    }

                }

            }

        }

        public static float GetDistanceFromPlayer(int p)
        {
            int playerPedId = GetPlayerPed(p);
            Vector3 playerCoords = GetEntityCoords(playerPedId, true, true);
            Vector3 myCoords = GetEntityCoords(PlayerPedId(), true, true);
            return GetDistanceBetweenCoords(myCoords.X, myCoords.Y, myCoords.Z, playerCoords.X, playerCoords.Y, playerCoords.Z, true);
        }
    }
}
