using Reactor.API.Logging.Base;
using Reactor.API.Extensions;
using Reactor.API.Logging.Sinks;

namespace Reactor.API.Logging.Decorators
{
    public class LogLevelDecorator : Decorator
    {
        public override string Decorate(LogLevel logLevel, string input, Sink sink)
        {
            return logLevel switch
            {
                LogLevel.Info => EncodeAnsiIfConsole("INF", sink, 255, 255, 255),
                LogLevel.Warning => EncodeAnsiIfConsole("WRN", sink, 255, 255, 0),
                LogLevel.Error => EncodeAnsiIfConsole("ERR", sink, 255, 0, 0),
                LogLevel.Debug => EncodeAnsiIfConsole("DBG", sink, 255, 0, 255),
                LogLevel.Exception => EncodeAnsiIfConsole("EXC", sink, 0, 255, 255),
                LogLevel.ReflectionTypeLoadException => EncodeAnsiIfConsole("RTL", sink, 0, 255, 255),

                _ => "???"
            };
        }

        private string EncodeAnsiIfConsole(string s, Sink sink, byte r, byte g, byte b)
        {
            if (sink is ConsoleSink)
                return s.AnsiColorEncodeRGB(r, g, b);

            return s;
        }
    }
}
