using Lusive.Events.Diagnostics;

namespace Vorp.Diagnostics
{
    public class EventLogger : IEventLogger
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
