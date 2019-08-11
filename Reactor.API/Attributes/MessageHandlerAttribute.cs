using System;

namespace Reactor.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class MessageHandlerAttribute : Attribute
    {
        public string SourceModID { get; }
        public string MessageName { get; }

        public MessageHandlerAttribute(string sourceModId, string messageName)
        {
            SourceModID = sourceModId;
            MessageName = messageName;
        }
    }
}
