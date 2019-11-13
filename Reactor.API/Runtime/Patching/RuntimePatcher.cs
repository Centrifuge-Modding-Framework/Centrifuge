using Harmony;
using Reactor.API.Logging;
using System;
using System.Reflection;

namespace Reactor.API.Runtime.Patching
{
    public static class RuntimePatcher
    {
        private static Log Log => LogManager.GetForInternalAssembly();

        public static HarmonyInstance HarmonyInstance { get; }

        static RuntimePatcher()
        {
            HarmonyInstance = HarmonyInstance.Create(Defaults.ReactorModLoaderNamespace);
        }

        public static void RunTranspilers()
        {
            var asm = Assembly.GetCallingAssembly();
            var types = asm.GetTypes();

            foreach (var type in types)
            {
                if (typeof(CodeTranspiler).IsAssignableFrom(type) && type != typeof(CodeTranspiler))
                {
                    var transpiler = Activator.CreateInstance(type) as CodeTranspiler;

                    Log.Info($"Transpiler: {type.FullName}");
                    try
                    {
                        transpiler.Apply(HarmonyInstance);
                    }
                    catch (Exception e)
                    {
                        Log.Exception(e);
                    }
                }
            }
        }

        public static void AutoPatch()
        {
            try
            {
                HarmonyInstance.PatchAll(Assembly.GetCallingAssembly());
            }
            catch
            {

            }
        }
    }
}
