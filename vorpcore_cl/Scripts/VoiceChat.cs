using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace vorpcore_cl.Scripts
{
    class VoiceChat : BaseScript
    {
        public static bool activeVoiceChat = false;
        public static List<float> voiceRange = new List<float>();
        public static int voiceRangeSelected = 0;

        public static uint keyRange = 0;

        //Tecla L
        public VoiceChat()
        {
            Tick += SetVoiceChat;
            Tick += StartVoiceChat;
        }

        private async Task StartVoiceChat()
        {
            if (Utils.GetConfig.isLoading && activeVoiceChat)
            {
                Function.Call((Hash)0x08797A8C03868CB8, voiceRange[voiceRangeSelected]);
                Function.Call((Hash)0xEC8703E4536A9952);
                Function.Call((Hash)0x58125B691F6827D5, voiceRange[voiceRangeSelected]);
            }
            await Delay(10000);
        }

        private async Task SetVoiceChat()
        {
            if (Utils.GetConfig.isLoading && activeVoiceChat)
            {
                if (API.IsControlJustPressed(0, keyRange))
                {
                    Debug.WriteLine(keyRange.ToString());
                    voiceRangeSelected = (voiceRangeSelected + 1) % voiceRange.Count;
                    TriggerEvent("vorp:TipRight", string.Format(Utils.GetConfig.Langs["VoiceRangeChanged"], voiceRange[voiceRangeSelected].ToString()), 4000);
                    Function.Call((Hash)0x08797A8C03868CB8, voiceRange[voiceRangeSelected]);
                    Function.Call((Hash)0xEC8703E4536A9952);
                    Function.Call((Hash)0x58125B691F6827D5, voiceRange[voiceRangeSelected]);
                }
                if (API.IsControlPressed(0, keyRange))
                {
                    Vector3 playerCoords = API.GetEntityCoords(API.PlayerPedId(), true, true);
                    Function.Call((Hash)0x2A32FAA57B937173, 0x94FDAE17, playerCoords.X, playerCoords.Y, playerCoords.Z - 0.5f, 0.0f, 0.0f, 0.0f, 0, 0.0f, 0.0f, voiceRange[voiceRangeSelected], voiceRange[voiceRangeSelected], 1.0, 255, 179, 38, 200, false, true, 2, false, false, false, false);
                }
            }
        }
    }
}
