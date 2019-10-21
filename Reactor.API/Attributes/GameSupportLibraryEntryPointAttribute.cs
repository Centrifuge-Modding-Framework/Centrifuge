using System;

namespace Reactor.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class GameSupportLibraryEntryPointAttribute : Attribute
    {
        public string LibraryID { get; }

        public GameSupportLibraryEntryPointAttribute(string libraryId)
        {
            LibraryID = libraryId;
        }
    }
}
