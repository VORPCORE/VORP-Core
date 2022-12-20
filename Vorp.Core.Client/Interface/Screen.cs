namespace Vorp.Core.Client.Interface
{
    internal class Screen
    {
        public static async Task FadeOut(int duration)
        {
            DoScreenFadeOut(duration);
            while (IsScreenFadingOut())
            {
                await BaseScript.Delay(100);
            }
        }
        public static async Task FadeIn(int duration)
        {
            DoScreenFadeIn(duration);
            while (IsScreenFadingIn())
            {
                await BaseScript.Delay(100);
            }
        }
    }
}
