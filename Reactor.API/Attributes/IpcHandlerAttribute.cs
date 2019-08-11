using System;

namespace Reactor.API.Attributes
{
    public class IpcHandlerAttribute : Attribute
    {
        public string ModIdentifier { get; set; }

        public IpcHandlerAttribute(string modIdentifier)
            => ModIdentifier = modIdentifier;
    }
}
