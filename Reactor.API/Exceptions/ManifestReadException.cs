using System;

namespace Reactor.API.Exceptions
{
    public class ManifestReadException : Exception
    {
        internal bool StreamFailed { get; }
        internal string Json { get; }

        public ManifestReadException(string message, bool streamFailed, string json = "", Exception innerException = null) : base(message, innerException)
        {
            StreamFailed = streamFailed;
            Json = json;
        }
    }
}
