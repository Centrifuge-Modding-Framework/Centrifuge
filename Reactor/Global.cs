using Reactor.API.Configuration;
using UnityEngine;

namespace Reactor
{
    internal static class Global
    {
        internal static GameObject GameApiObject { get; set; }
        internal static Settings Settings { get; set; }

        internal const string InterceptUnityLogsSettingsKey = "InterceptUnityLogs";
    }
}
