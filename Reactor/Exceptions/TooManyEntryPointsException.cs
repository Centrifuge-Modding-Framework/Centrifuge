using System;

namespace Reactor.Exceptions
{
    public class TooManyEntryPointsException : Exception
    {
        public TooManyEntryPointsException(string message) : base(message) { }
    }
}
