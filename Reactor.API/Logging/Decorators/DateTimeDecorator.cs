using Reactor.API.Logging.Base;
using Reactor.API.Extensions;
using System;
using Reactor.API.Logging.Sinks;

namespace Reactor.API.Logging.Decorators
{
    public class DateTimeDecorator : Decorator
    {
        public override string Decorate(LogLevel logLevel, string input, Sink sink)
        {
            var dateTimeString = DateTime.Now.ToString();

            if (sink is ConsoleSink)
                return dateTimeString.AnsiColorEncodeRGB(63, 63, 63);

            return dateTimeString;
        }
    }
}
