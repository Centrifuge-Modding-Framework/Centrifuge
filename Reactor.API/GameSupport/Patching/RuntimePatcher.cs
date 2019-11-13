using Harmony;
using Reactor.API.Logging;
using System;
using System.Reflection;

namespace Reactor.API.GameSupport.Patching
{
    public static class RuntimePatcher
    {
        private static Log Log { get; }
        public static HarmonyInstance HarmonyInstance { get; }

        static RuntimePatcher()
        {
            HarmonyInstance = HarmonyInstance.Create(Defaults.ReactorModLoaderNamespace);
            Log = new Log("RuntimePatcher");
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
                    transpiler.Apply(HarmonyInstance);
                }
            }
        }

        public static void AutoPatch()
        {
            HarmonyInstance.PatchAll(Assembly.GetCallingAssembly());
        }
    }
}
