﻿using Harmony;
using Reactor.API.GTTOD.Infrastructure;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Reactor.API.GTTOD.Transpilers
{
    internal static partial class EnemyNPC
    {
        private class DroneDie : GameCodeTranspiler
        {
            private const int EventHookOpIndex = 27;

            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
            {
                var modified = new List<CodeInstruction>(instr);

                var invoker = typeof(Events.EnemyNPC).GetMethod(
                    nameof(Events.EnemyNPC.InvokeDroneDied),
                    BindingFlags.NonPublic | BindingFlags.Static
                );

                modified.Insert(EventHookOpIndex, new CodeInstruction(OpCodes.Call, invoker));
                modified.Insert(EventHookOpIndex, new CodeInstruction(OpCodes.Ldarg_0));

                return modified;
            }

            public override void Apply(HarmonyInstance harmony)
            {
                var targetMethod = typeof(Drone).GetMethod(
                    nameof(Drone.Die),
                    BindingFlags.Public | BindingFlags.Instance
                );

                var transpilerMethod = typeof(DroneDie).GetMethod(
                    nameof(Transpiler),
                    BindingFlags.NonPublic | BindingFlags.Static
                );

                harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
            }
        }
    }
}
