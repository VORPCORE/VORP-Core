using System.Threading.Tasks;

namespace Vorp.Core.Client
{
    internal class Session
    {
        public static async Task Loading()
        {
            while (true)
            {
                if (NetworkIsPlayerActive(PlayerId())) return;

                await BaseScript.Delay(1000);
            }
        }
    }
}
