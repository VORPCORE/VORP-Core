using Lusive.Events.Diagnostics;

namespace Vorp.Diagnostics
{
    public class EventLogger : IEventLogger
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

        public void Debug(params object[] values)
        {
            if (ShowOutput("debug"))
                CitizenFX.Core.Debug.WriteLine(Format(values));
        }

        public void Info(params object[] values)
        {
            if (ShowOutput("info"))
                CitizenFX.Core.Debug.WriteLine(Format(values));
        }

        public void Error(params object[] values)
        {
            if (ShowOutput("error"))
                CitizenFX.Core.Debug.WriteLine(Format(values));
        }

        public string Format(object[] values)
        {
            return $"[Events] {string.Join(", ", values)}";
        }
    }
}
