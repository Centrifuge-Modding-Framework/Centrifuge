using Reactor.API.Configuration;
using UnityEngine;

namespace Reactor
{
    internal static class Global
    {
        internal const string InterceptUnityLogsSettingsKey = "InterceptUnityLogs";

        internal static GameObject GameApiObject { get; set; }
        internal static Settings Settings { get; set; }
    }
}
