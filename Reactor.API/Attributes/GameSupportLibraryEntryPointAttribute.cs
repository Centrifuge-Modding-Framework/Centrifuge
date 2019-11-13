using System;

namespace Reactor.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class GameSupportLibraryEntryPointAttribute : Attribute
    {
        public string LibraryID { get; }
        public string InitializerName { get; } = "Initialize";
        public bool AwakeAfterInitialize { get; } = true;

        public GameSupportLibraryEntryPointAttribute(string libraryId)
        {
            LibraryID = libraryId;
        }
    }
}
