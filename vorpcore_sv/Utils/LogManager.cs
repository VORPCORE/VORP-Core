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
            switch (type)
            {
                case "warning": // Warning Yellow
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(msg);
                    break;
                case "error": // Error Red
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(msg);
                    break;
                case "success": // Success Green
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(msg);
                    break;
                case "info": // Info Blue Cyan
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(msg);
                    break;
                default: // Normal
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(msg);
                    break;
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

    }
}
