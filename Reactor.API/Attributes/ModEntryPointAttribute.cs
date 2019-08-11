using System;

namespace Reactor.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ModEntryPointAttribute : Attribute
    {
        public string ModID { get; }
        public string InitializerName { get; set; } = "Initialize";

        public ModEntryPointAttribute(string modId)
        {
            ModID = modId;
        }
    }
}
