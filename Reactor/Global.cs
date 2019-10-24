﻿using Reactor.API.Configuration;

namespace Reactor
{
    internal class Global
    {
        internal const string InterceptUnityLogsSettingsKey = "InterceptUnityLogs";

        internal static bool InterceptUnityLogs { get; set; }

        internal static object GameApiObject { get; set; }
        internal static Settings Settings { get; set; }
    }
}
