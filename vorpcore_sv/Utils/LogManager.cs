using CitizenFX.Core;
using System;
using System.Threading.Tasks;

namespace vorpcore_sv.Utils
{
    public class LogManager : BaseScript
    {
        public LogManager()
        {

        }

        public static void WriteLog(string msg, string type)
        {
            Debug.WriteLine($"[{type}] {msg}");
        }

    }
}
