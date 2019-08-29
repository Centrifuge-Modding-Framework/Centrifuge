using Harmony;
using Reactor.API.GTTOD.Infrastructure;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Reactor.API.GTTOD.Transpilers
{
    internal static partial class Game
    {
        private class StartGame : GameCodeTranspiler
        {
            private const int EventHookOpCodeIndex = 107;
            private const string StartGameCoroutineClassName = "<StartGame>d__78";

            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
            {
                var modified = new List<CodeInstruction>(instr);

                var invoker = typeof(Events.Game).GetMethod(
                    nameof(Events.Game.InvokeGameModeStarted),
                    BindingFlags.NonPublic | BindingFlags.Static
                );

                modified.Insert(EventHookOpCodeIndex, new CodeInstruction(OpCodes.Call, invoker));

                return modified;
            }

            public override void Apply(HarmonyInstance harmony)
            {
                var targetMethod = typeof(global::GTTODManager).GetNestedType(
                    StartGameCoroutineClassName,
                    BindingFlags.NonPublic
                ).GetMethod(
                    "MoveNext",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );

                var transpilerMethod = typeof(StartGame).GetMethod(
                    nameof(Transpiler),
                    BindingFlags.NonPublic | BindingFlags.Static
                );

                harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
            }
        }
    }
}
