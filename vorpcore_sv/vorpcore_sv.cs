using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using vorpcore_sv.Scripts;
using vorpcore_sv.Utils;

namespace vorpcore_sv
{
    public class vorpcore_sv : BaseScript
    {
        public static PlayerList PlayerList;
        public static vorpcore_sv Instance;

        public vorpcore_sv()
        {
            PlayerList = Players;

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(@"" + "\n" +
                            @" /$$    /$$  /$$$$$$  /$$$$$$$  /$$$$$$$   /$$$$$$    /$$ /$$   /$$$$$$$  /$$$$$$$$" + "\n" +
                            @"| $$   | $$ /$$__  $$| $$__  $$| $$__  $$ /$$__  $$  / $$/ $$  | $$__  $$| $$_____/" + "\n" +
                            @"| $$   | $$| $$  \ $$| $$  \ $$| $$  \ $$| $$  \__/ /$$$$$$$$$$| $$  \ $$| $$      " + "\n" +
                            @"|  $$ / $$/| $$  | $$| $$$$$$$/| $$$$$$$/| $$      |   $$  $$_/| $$$$$$$/| $$$$$   " + "\n" +
                            @" \  $$ $$/ | $$  | $$| $$__  $$| $$____/ | $$       /$$$$$$$$$$| $$__  $$| $$__/   " + "\n" +
                            @"  \  $$$/  | $$  | $$| $$  \ $$| $$      | $$    $$|_  $$  $$_/| $$  \ $$| $$      " + "\n" +
                            @"   \  $/   |  $$$$$$/| $$  | $$| $$      |  $$$$$$/  | $$| $$  | $$  | $$| $$$$$$$$" + "\n" +
                            @"    \_/     \______/ |__/  |__/|__/       \______/   |__/|__/  |__/  |__/|________/" + "\n" +
                            "");

            if (IsLinux)
            {
                Console.WriteLine("\nVORP CORE Running on Linux, thanks for using VorpCore");
            }
            Console.ForegroundColor = ConsoleColor.White;

            Instance = this;
        }

        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

    }

}
