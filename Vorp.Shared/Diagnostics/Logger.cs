namespace Vorp.Diagnostics
{
    internal class Logger
    {
        public static void Debug(string msg)
        {
            WriteLogMessage($"[DEBUG] {msg}");
        }

        private static void WriteLogMessage(string msg)
        {
            CitizenFX.Core.Debug.WriteLine(msg);
        }
    }
}
