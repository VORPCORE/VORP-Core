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

        public void Start() => PluginManager.Instance.AttachTickHandler(OnFreezeTime);
        public void Stop() => PluginManager.Instance.DetachTickHandler(OnFreezeTime);

        private async Task OnFreezeTime()
        {
            NetworkClockTimeOverride(Hour, Minute, 0, 0, true);
            SetClockTime(Hour, Minute, 0);
            PauseClock(true, 0);
        }
    }
}
