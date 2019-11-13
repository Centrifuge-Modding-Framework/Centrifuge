using Reactor.API.Logging.Base;

namespace Reactor.API.Logging.Decorators
{
    public class LogLevelDecorator : Decorator
    {
        public override string Decorate(LogLevel logLevel, string input)
        {
            return logLevel switch
            {
                LogLevel.Info => "INF",
                LogLevel.Warning => "WRN",
                LogLevel.Error => "ERR",
                LogLevel.Debug => "DBG",
                LogLevel.Exception => "EXC",
                LogLevel.ReflectionTypeLoadException => "RTL",

                _ => "???"
            };
        }
    }
}
