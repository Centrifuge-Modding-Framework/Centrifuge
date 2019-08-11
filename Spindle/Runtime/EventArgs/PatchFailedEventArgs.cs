using System;

namespace Spindle.Runtime.EventArgs
{
    public class PatchFailedEventArgs : System.EventArgs
    {
        public string Name { get; private set; }
        public Exception Exception { get; private set; }

        public PatchFailedEventArgs(string name, Exception exception)
        {
            Name = name;
            Exception = exception;
        }
    }
}
