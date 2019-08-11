using System.Collections.Generic;

namespace Reactor.API.DataModel
{
    public class ModInfo
    {
        public string ModID { get; }

        public string Name { get; }
        public string Author { get; }
        public string Contact { get; }
        public int Priority { get; }

        public List<string> Dependencies { get; }

        public ModInfo(string modId, string name, string author, string contact, int priority)
        {
            ModID = modId;

            Name = name;
            Author = author;
            Contact = contact;
            Priority = priority;
        }
    }
}
