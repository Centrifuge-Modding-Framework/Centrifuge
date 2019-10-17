using Reactor.API.Configuration;

namespace Reactor
{
    public static class Global
    {
        public static bool InterceptUnityLogs;

        internal const string InterceptUnityLogsSettingsKey = "InterceptUnityLogs";

        internal static object GameApiObject { get; set; }
        internal static Settings Settings { get; set; }

    }
}
