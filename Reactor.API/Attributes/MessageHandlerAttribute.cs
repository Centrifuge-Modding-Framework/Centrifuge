using System;

namespace Reactor.API.Attributes
{
    [Obsolete("Use IManager.GetMod(string modId).Instance to communicate between the mods instead." +
              "\nThis will be removed in Centrifuge 4.0.")]
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
