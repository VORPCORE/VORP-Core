using System.Threading.Tasks;

namespace Vorp.Core.Client
{
    internal class Session
    {
        public static bool IsLoaded = false;

        public static async Task Loading()
        {
            while (true)
            {
                if (NetworkIsPlayerActive(PlayerId())) return;

                await BaseScript.Delay(1000);
            }
            IsLoaded = true;
        }

        /// <summary>
        /// Players server handle
        /// </summary>
        public static int ServerId => GetPlayerServerId(PlayerId());
    }
}
