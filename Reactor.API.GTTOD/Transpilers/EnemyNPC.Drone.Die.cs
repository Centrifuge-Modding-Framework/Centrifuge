using Harmony;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Reactor.API.GTTOD.Transpilers
{
    internal static partial class EnemyNPC
    {
        private static class DroneDie
        {
            private const int EventHookOpIndex = 24;

            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
            {
                var modified = new List<CodeInstruction>(instr);
                var invoker = typeof(Events.EnemyNPC).GetMethod(nameof(Events.EnemyNPC.InvokeDroneDied), BindingFlags.NonPublic | BindingFlags.Static);

                modified.Insert(EventHookOpIndex, new CodeInstruction(OpCodes.Call, invoker));
                modified.Insert(EventHookOpIndex, new CodeInstruction(OpCodes.Ldarg_0));

                return modified;
            }

            public static void Apply(HarmonyInstance harmony)
            {
                var methodInfo = typeof(Drone).GetMethod(nameof(Drone.Die), BindingFlags.Public | BindingFlags.Instance);
                var transpilerMethod = typeof(DroneDie).GetMethod(nameof(Transpiler), BindingFlags.NonPublic | BindingFlags.Static);

                harmony.Patch(methodInfo, transpiler: new HarmonyMethod(transpilerMethod));
            }
        }
    }
}
