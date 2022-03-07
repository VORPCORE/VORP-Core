namespace Vorp.Shared.Diagnostics
{
    public static class Logger
    {
#if CLIENT
        static string _loggingLevel = GetResourceMetadata(GetCurrentResourceName(), "log_level", 0);
#elif SERVER
        static string _loggingLevel = GetResourceMetadata(GetCurrentResourceName(), "log_level", 0);
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

        public static void Trace(string msg)
        {
            if (ShowOutput("trace"))
                Format($"[TRACE] {msg}");
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
