using Reactor.API.Configuration;
using System.Collections.Generic;

namespace Reactor
{
    internal class Global
    {
        internal const string InterceptUnityLogsSettingsKey = "InterceptUnityLogs";

        internal static bool InterceptUnityLogs { get; set; }

        internal static Dictionary<string, object> GameApiObjects { get; set; }
        internal static Settings Settings { get; set; }

        static Global()
        {
            GameApiObjects = new Dictionary<string, object>();
        }
    }
}
