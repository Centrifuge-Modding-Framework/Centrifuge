namespace Reactor.API.DataModel
{
    public class ModInfo
    {
        public string Name { get; }
        public string Author { get; }
        public string AuthorContact { get; }
        public string IPCIdentifier { get; }
        public int Priority { get; }

        public ModInfo(string name, string author, string authorContact, string ipcIdentifier, int priority)
        {
            Name = name;
            Author = author;
            AuthorContact = authorContact;
            IPCIdentifier = ipcIdentifier;
            Priority = priority;
        }
    }
}
