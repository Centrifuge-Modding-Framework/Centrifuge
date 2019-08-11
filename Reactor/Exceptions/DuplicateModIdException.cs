using System;

namespace Reactor.Exceptions
{
    public class DuplicateModIdException : Exception
    {
        public string ModID { get; }

        public DuplicateModIdException(string modId, string message) : base(message)
        {
            ModID = modId;
        }
    }
}
