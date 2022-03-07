using Vorp.Shared.Enums;

namespace Vorp.Core.Client.RedM
{
    internal class World
    {
        public static void SetWeather(eWeatherType weatherType, bool transition = false, float transitionDuration = 0f) => Function.Call((Hash)0x2C6A07AF9AEDABD8, (long)weatherType, false, false, transition, transitionDuration, false);
        public static void SetWeatherFrozen(bool frozen) => Function.Call((Hash)0xD74ACDF7DB8114AF, frozen);

        public static float WindSpeed
        {
            get => Function.Call<float>((Hash)0xFFB7E74E041150A4);
            set => Function.Call((Hash)0xD00C2D82DC04A99F, value);
        }
    }
}
