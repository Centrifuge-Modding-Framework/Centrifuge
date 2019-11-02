using System;

namespace Reactor.API.Logging
{
    [Flags]
    public enum LogToggles
    {
        Info,
        Warning,
        Error,
        Exception,
    }
}
