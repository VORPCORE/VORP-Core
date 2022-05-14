namespace Vorp.Shared
{
    public class Common
    {
        public static async void MoveToMainThread()
        {
            await BaseScript.Delay(0);
        }
    }
}
