namespace Vorp.Core.Client.RedM
{
    internal class WorldTime
    {
        public int Hour = 0;
        public int Minute = 0;

        public WorldTime(int hour, int minute, bool freezeNow = true)
        {
            Hour = hour;
            Minute = minute;

            if (freezeNow)
                PluginManager.Instance.AttachTickHandler(OnFreezeTime);
        }

        public bool IsClockTimeOverridden => Function.Call<bool>((Hash)0xD7C95D322FF57522);
        public void ClearClockTimeOverride() => Function.Call((Hash)0xD972DF67326F966E);

        // NetworkClockTimeOverride_2
        public void ClockTimeOverride(int hour, int minute, int transitionTime = 0, bool pauseClock = false, bool clockwise = false) => Function.Call((Hash)0xE28C13ECC36FF14E, hour, minute, 0, transitionTime, pauseClock, clockwise);

        public void Start() => PluginManager.Instance.AttachTickHandler(OnFreezeTime);

        public void Stop()
        {
            if (IsClockTimeOverridden)
                ClearClockTimeOverride();

            PluginManager.Instance.DetachTickHandler(OnFreezeTime);
        }

        private async Task OnFreezeTime()
        {
            ClockTimeOverride(Hour, Minute, 0, true, true);
            SetClockTime(Hour, Minute, 0);
            PauseClock(true, 0);
        }
    }
}
