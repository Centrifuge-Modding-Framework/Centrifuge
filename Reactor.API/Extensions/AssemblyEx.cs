using System;
using System.Linq;
using System.Reflection;

namespace Reactor.API.Extensions
{
    public static class AssemblyEx
    {
        public static Assembly GetAssemblyByName(string name)
            => AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(asm => asm.GetName().Name == name);
    }
}
