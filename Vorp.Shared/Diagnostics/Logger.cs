using System;

namespace Vorp.Shared.Diagnostics
{
    public static class Logger
    {
        public static void Info(string msg)
        {
            Format($"[INFO] {msg}");
        }

        public static void Success(string msg)
        {
            Format($"[SUCCESS] {msg}");
        }

        public static void Warn(string msg)
        {
            Format($"[WARN] {msg}");
        }

        public static void Debug(string msg)
        {
            if (GetConvarInt("vorp_debug", 0) == 1)
                Format($"[DEBUG] {msg}");
        }

        public static void Error(string msg)
        {
            Format($"[ERROR] {msg}");
        }

        public static void Error(Exception ex, string msg)
        {
            Format($"[ERROR] {msg}\r\n{ex}");
        }

        static void Format(string msg)
        {
            CitizenFX.Core.Debug.WriteLine($"[VORP-CORE] {msg}");
        }
    }
}
