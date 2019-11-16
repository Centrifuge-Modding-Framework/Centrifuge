using System;
using System.Linq;

namespace Centrifuge.UnityInterop
{
    internal static class Kernel
    {
        internal static Type FindTypeByFullName(string fullName, string assemblyFilter)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                                    .Where(a => a.GetName().Name.Contains(assemblyFilter));

            foreach (var asm in assemblies)
            {
                var type = asm.GetTypes().FirstOrDefault(t => t.FullName == fullName);

                if (type == null)
                    continue;

                return type;
            }

            throw new Exception($"Type {fullName} wasn't found in the main AppDomain at this moment.");
        }
    }
}
