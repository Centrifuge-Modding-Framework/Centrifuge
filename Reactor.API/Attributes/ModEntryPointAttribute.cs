using System;

namespace Reactor.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ModEntryPointAttribute : Attribute
    {
        public string ModID { get; }
        public string InitializerName { get; set; } = "Initialize";
        public string LoaderMethodName { get; set; } = "Load";

        public bool AwakeAfterInitialize = false;

        public ModEntryPointAttribute(string modId)
        {
            ModID = modId;
        }
    }
}
