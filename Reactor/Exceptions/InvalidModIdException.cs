using System;

namespace Reactor.Exceptions
{
    public class InvalidModIdException : Exception
    {
        public InvalidModIdException(string message) : base(message) { }
    }
}
