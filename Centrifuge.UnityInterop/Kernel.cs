using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Centrifuge.UnityInterop
{
    internal static class Kernel
    {
        public static Type FindTypeByFullName(string fullName)
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

        // Currently unused.
        public static void LoadUnityAssembly(string asmName)
        {
            try
            {
                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var dllPath = Path.Combine(location, asmName);

                if (File.Exists(dllPath))
                    Assembly.LoadFrom(dllPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
