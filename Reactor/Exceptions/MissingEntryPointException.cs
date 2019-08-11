using System;

namespace Reactor.Exceptions
{
    public class MissingEntryPointException : Exception
    {
        public MissingEntryPointException(string message) : base(message) { }
    }
}
