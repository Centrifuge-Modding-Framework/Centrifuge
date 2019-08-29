﻿using Harmony;
using Reactor.API.GTTOD.Events;
using Reactor.API.GTTOD.Infrastructure;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Reactor.API.GTTOD.Transpilers
{
    internal static partial class WeaponScript
    {
        private class PrimaryFire : GameCodeTranspiler
        {
            private const int EventHookOpCodeIndex = 130;
            private const string PrimaryFireCoroutineClassName = "<PrimaryFire>d__139";

            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
            {
                var modified = new List<CodeInstruction>(instr);

                var bullet = typeof(global::WeaponScript).GetField(
                    nameof(global::WeaponScript.Bullet),
                    BindingFlags.NonPublic | BindingFlags.Instance
                );

                var invoker = typeof(Weapon).GetMethod(
                    nameof(Weapon.InvokeShotFiredPrimary),
                    BindingFlags.NonPublic | BindingFlags.Static
                );

                modified.Insert(EventHookOpCodeIndex, new CodeInstruction(OpCodes.Call, invoker));
                modified.Insert(EventHookOpCodeIndex, new CodeInstruction(OpCodes.Ldloc_1));

                return modified;
            }

            public override void Apply(HarmonyInstance harmony)
            {
                var targetMethod = typeof(global::WeaponScript).GetNestedType(
                    PrimaryFireCoroutineClassName,
                    BindingFlags.NonPublic
                ).GetMethod(
                    "MoveNext",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );

                var transpilerMethod = typeof(PrimaryFire).GetMethod(
                    nameof(Transpiler),
                    BindingFlags.NonPublic | BindingFlags.Static
                );

                harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
            }
        }
    }
}
