using System.Threading.Tasks;

namespace Vorp.Shared
{
    public class Common
    {
        public static async Task MoveToMainThread()
        {
            await BaseScript.Delay(0);
        }
    }
}
