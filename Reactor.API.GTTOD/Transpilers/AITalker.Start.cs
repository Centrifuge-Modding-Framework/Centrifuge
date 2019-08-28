using Harmony;
using Reactor.API.GTTOD.Infrastructure;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Reactor.API.GTTOD.Transpilers
{
    internal static partial class AITalker
    {
        private class Start : GameCodeTranspiler
        {
            // After GM.Talkers.Add(this);
            private const int OpCodeInsertionIndex = 5;

            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
            {
                var modified = new List<CodeInstruction>(instr);

                var initMethod = typeof(EnemyChatter).GetMethod(
                    nameof(EnemyChatter.InitializeAdditionalTalkerMessages),
                    BindingFlags.NonPublic | BindingFlags.Static
                );

                modified.Insert(OpCodeInsertionIndex, new CodeInstruction(OpCodes.Call, initMethod));
                modified.Insert(OpCodeInsertionIndex, new CodeInstruction(OpCodes.Ldarg_0));

                return modified;
            }

            public override void Apply(HarmonyInstance harmony)
            {
                var targetMethod = typeof(global::AITalker).GetMethod(
                    nameof(global::AITalker.Start),
                    BindingFlags.NonPublic | BindingFlags.Instance
                );

                var transpilerMethod = typeof(Start).GetMethod(
                    nameof(Transpiler),
                    BindingFlags.NonPublic | BindingFlags.Static
                );

                harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
            }
        }
    }
}
