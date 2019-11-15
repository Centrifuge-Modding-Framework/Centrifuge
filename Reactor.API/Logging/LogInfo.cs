using System.Reflection;

namespace Reactor.API.Logging
{
    internal class LogInfo
    {
        internal Assembly OwningAssembly { get; set; }
        internal Log Log { get; set; }
    }
}
