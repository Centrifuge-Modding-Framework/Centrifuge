using Harmony;
using Reactor.API.Logging;
using System;
using System.Reflection;

namespace Reactor.API.GameSupport.Patching
{
    public static class RuntimePatcher
    {
        public static HarmonyInstance HarmonyInstance { get; }
        
        private static Log Log { get; }

        static RuntimePatcher()
        {
            HarmonyInstance = HarmonyInstance.Create(Defaults.ReactorModLoaderNamespace);
            Log = new Log("RuntimePatcher");
        }

        public static void InitializeTranspilers()
        {
            var asm = Assembly.GetCallingAssembly();
            var types = asm.GetTypes();

            foreach (var type in types)
            {
                if (typeof(GameCodeTranspiler).IsAssignableFrom(type) && type != typeof(GameCodeTranspiler))
                {
                    var transpiler = Activator.CreateInstance(type) as GameCodeTranspiler;

                    Log.Info($"Transpiler: {type.FullName}");
                    transpiler.Apply(HarmonyInstance);
                }
            }
        }
    }
}
