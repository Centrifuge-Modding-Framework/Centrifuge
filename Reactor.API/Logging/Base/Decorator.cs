namespace Reactor.API.Logging.Base
{
    public abstract class Decorator
    {
        public abstract string Decorate(LogLevel logLevel, string input, Sink sink);
    }
}
