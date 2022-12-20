using Vorp.Shared.Enums;

namespace Vorp.Core.Client.RedM
{
    internal class World
    {
        public static void SetWeather(eWeatherType weatherType, bool transition = false, float transitionDuration = 0f) => Function.Call((Hash)0x2C6A07AF9AEDABD8, (long)weatherType, true, false, transition, transitionDuration, false);
        public static void SetWeatherTransition(eWeatherType weatherType, eWeatherType weatherType2, float transition = 0f) => Function.Call((Hash)0xFA3E3CA8A1DE6D5D, (long)weatherType, (long)weatherType2, transition, true);
        public static void SetWeatherFrozen(bool frozen) => Function.Call((Hash)0xD74ACDF7DB8114AF, frozen);

        public static float WindSpeed
        {
            get => Function.Call<float>((Hash)0xFFB7E74E041150A4);
            set => Function.Call((Hash)0xD00C2D82DC04A99F, value);
        }

        public static void SetWindDirection(float direction) => Function.Call((Hash)0xB56C4F5F57A45600, direction);
        public static Vector3 GetWindDirection => Function.Call<Vector3>((Hash)0xF703E82F3FE14A5F);
    }
}
