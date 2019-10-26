using System;
using System.Linq;

namespace Centrifuge.UnityInterop
{
    internal static class Kernel
    {
        internal static Type FindTypeByFullName(string fullName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var asm in assemblies)
            {
                var type = asm.GetTypes().FirstOrDefault(t => t.FullName == fullName);

                if (type == null)
                    continue;

                return type;
            }

            throw new Exception($"Type {fullName} wasn't found at this moment in the main AppDomain.");
        }
    }
}
