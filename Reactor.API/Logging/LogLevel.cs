using System;

namespace Reactor.API.Logging
{
    [Flags]
    public enum LogLevel
    {
        Info = 1,
        Warning = 2,
        Error = 4,
        Debug = 8,
        Exception = 16,
        ReflectionTypeLoadException = 32,
        Everything = Info | Warning | Error | Debug | Exception | ReflectionTypeLoadException
    }
}
