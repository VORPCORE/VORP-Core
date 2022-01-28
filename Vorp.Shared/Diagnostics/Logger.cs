using Lusive.Events.Diagnostics;

namespace Vorp.Diagnostics
{
    internal class Logger : IEventLogger
    {
        public void Debug(params object[] values)
        {
            CitizenFX.Core.Debug.WriteLine(Format(values));
        }

        public void Info(params object[] values)
        {
            CitizenFX.Core.Debug.WriteLine(Format(values));
        }

        public void Error(params object[] values)
        {
            CitizenFX.Core.Debug.WriteLine(Format(values));
        }

        public string Format(object[] values)
        {
            return $"[Events] {string.Join(", ", values)}";
        }
    }
}
