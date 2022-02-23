namespace Vorp.Core.Client
{
    internal class Session
    {
        public static bool IsLoaded = false;

        public static bool HasSelectedCharacter = false;

        public static async Task Loading()
        {
            while (true)
            {
                if (NetworkIsSessionStarted()) break;

                await BaseScript.Delay(500);
            }
            IsLoaded = true;
        }

        /// <summary>
        /// Players server handle
        /// </summary>
        public static int ServerId => GetPlayerServerId(PlayerId());
    }
}
