using Reactor.API.Logging.Base;
using Reactor.API.Logging.Sinks;
using Reactor.API.Extensions;

namespace Reactor.API.Logging.Decorators
{
    public class MessageOutputDecorator : Decorator
    {
        public override string Decorate(LogLevel logLevel, string input, string originalMessage, Sink sink)
        {
            var output = originalMessage;

            if (sink is ConsoleSink)
                output = originalMessage.AnsiColorEncodeRGB(255, 255, 255);

            return output;
        }
    }
}
