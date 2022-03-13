namespace Vorp.Core.Client.RedM
{
    public class WorldTime
    {
        public WorldTime()
        {

        }

        public bool IsClockTimeOverridden => Function.Call<bool>((Hash)0xD7C95D322FF57522);
        public void ClearClockTimeOverride() => Function.Call((Hash)0xD972DF67326F966E);

        // NetworkClockTimeOverride_2
        public void ClockTimeOverride(int hour, int minute, int transitionTime = 0, bool pauseClock = false, bool clockwise = false) => Function.Call((Hash)0xE28C13ECC36FF14E, hour, minute, 0, transitionTime, pauseClock, clockwise);
    }
}
