namespace Reactor.API.Logging
{
    public class LogOptions
    {
        public bool WriteToConsole { get; set; }
        public bool ColorizeLines { get; set; }
        public LogToggles Toggles { get; set; }

        public LogOptions()
        {
            WriteToConsole = true;
            ColorizeLines = true;
            Toggles = LogToggles.Info | LogToggles.Warning | LogToggles.Error | LogToggles.Exception;
        }
    }
}
