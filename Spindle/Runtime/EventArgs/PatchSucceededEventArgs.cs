namespace Spindle.Runtime.EventArgs
{
    public class PatchSucceededEventArgs : System.EventArgs
    {
        public string Name { get; set; }

        public PatchSucceededEventArgs(string name)
        {
            Name = name;
        }
    }
}
