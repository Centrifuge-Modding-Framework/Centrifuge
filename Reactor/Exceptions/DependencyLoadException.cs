using System;

namespace Reactor.Exceptions
{
    internal class DependencyLoadException : Exception
    {
        public string DependencyFileName { get; }

        public DependencyLoadException(string dependencyFileName, string message) : base(message)
        {
            DependencyFileName = dependencyFileName;
        }
    }
}
