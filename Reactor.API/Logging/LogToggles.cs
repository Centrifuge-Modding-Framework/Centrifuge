using System;

namespace Reactor.API.Logging
{
    [Flags]
    public enum LogToggles
    {
        Info = 1,
        Warning = 2,
        Error = 4,
        Exception = 8,
    }
}
