using Reactor.API.Logging.Base;
using System;

namespace Reactor.API.Logging.Decorators
{
    public class DateTimeDecorator : Decorator
    {
        public override string Decorate(LogLevel logLevel, string input)
        {
            return DateTime.Now.ToString();
        }
    }
}
