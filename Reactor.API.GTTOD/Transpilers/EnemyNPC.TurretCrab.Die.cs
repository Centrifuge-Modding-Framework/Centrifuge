using Harmony;
using Reactor.API.GTTOD.Infrastructure;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Reactor.API.GTTOD.Transpilers
{
    internal static partial class EnemyNPC
    {
        private class TurretCrabDie : GameCodeTranspiler
        {
            private const int EventHookOpIndex = 41;

            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
            {
                var modified = new List<CodeInstruction>(instr);
                var invoker = typeof(Events.EnemyNPC).GetMethod(nameof(Events.EnemyNPC.InvokeTurretCrabDied), BindingFlags.NonPublic | BindingFlags.Static);

                modified.Insert(EventHookOpIndex, new CodeInstruction(OpCodes.Call, invoker));
                modified.Insert(EventHookOpIndex, new CodeInstruction(OpCodes.Ldarg_0));

                return modified;
            }

            public override void Apply(HarmonyInstance harmony)
            {
                var methodInfo = typeof(TurretCrab).GetMethod(nameof(TurretCrab.Die), BindingFlags.Public | BindingFlags.Instance);
                var transpilerMethod = typeof(TurretCrabDie).GetMethod(nameof(Transpiler), BindingFlags.NonPublic | BindingFlags.Static);

                harmony.Patch(methodInfo, transpiler: new HarmonyMethod(transpilerMethod));
            }
        }
    }
}
