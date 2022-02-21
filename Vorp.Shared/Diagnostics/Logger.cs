using System;

namespace Vorp.Shared.Diagnostics
{
    public static class Logger
    {
#if CLIENT
        static string _loggingLevel = GetConvar2("vorp_logging_level", "none");
#elif SERVER
        static string _loggingLevel = GetConvar("vorp_logging_level", "none");
#endif

        static bool ShowOutput(string level)
        {
            string lowercase = _loggingLevel.ToLower();
            if (lowercase == "all") return true;
            return (lowercase == _loggingLevel);
        }

        public static void Info(string msg)
        {
            if (ShowOutput("info"))
                Format($"[INFO] {msg}");
        }

        public static void Success(string msg)
        {
            if (ShowOutput("trace"))
                Format($"[SUCCESS] {msg}");
        }

        public static void Warn(string msg)
        {
            if (ShowOutput("warn"))
                Format($"[WARN] {msg}");
        }

        public static void Debug(string msg)
        {
            if (ShowOutput("debug"))
                Format($"[DEBUG] {msg}");
        }

        public static void Error(string msg)
        {
            if (ShowOutput("error"))
                Format($"[ERROR] {msg}");
        }

        public static void Error(Exception ex, string msg)
        {
            if (ShowOutput("error"))
                Format($"[ERROR] {msg}\r\n{ex}");
        }

        static void Format(string msg)
        {
            CitizenFX.Core.Debug.WriteLine($"{msg}");
        }
    }
}
