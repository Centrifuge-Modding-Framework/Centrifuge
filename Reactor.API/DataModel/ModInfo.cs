using System.Collections.Generic;

namespace Reactor.API.DataModel
{
    public class ModInfo
    {
        public string ModID { get; }
        public object Instance { get; }
        public bool IsMonoBehaviour { get; }

        public string Name { get; }
        public string Author { get; }
        public string Contact { get; }
        public int Priority { get; }

        public List<string> Dependencies { get; }

        public ModInfo(
            string modId,
            object instance, 
            bool isMonoBehaviour, 
            string name,
            string author,
            string contact,
            int priority,
            List<string> dependencies)
        {
            ModID = modId;
            Instance = instance;
            IsMonoBehaviour = isMonoBehaviour;

            Name = name;
            Author = author;
            Contact = contact;
            Priority = priority;
            Dependencies = dependencies;
        }
    }
}