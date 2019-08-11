using Reactor.API.Configuration;

namespace Reactor
{
    internal static class Global
    {
        internal static Settings Settings { get; set; }

        internal const string InterceptUnityLogsSettingsKey = "InterceptUnityLogs";
    }
}
