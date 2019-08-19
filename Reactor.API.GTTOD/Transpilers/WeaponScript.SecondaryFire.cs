﻿using Harmony;
using Reactor.API.GTTOD.Events;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Reactor.API.GTTOD.Transpilers
{
    internal static partial class WeaponScript
    {
        private static class SecondaryFire
        {
            private const int EventHookOpCodeIndex = 91;
            private const int PrevBranchOpCodeIndex = 93;
            private const string SecondaryFireCoroutineClassName = "<SecondaryFire>d__130";

            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
            {
                var modified = new List<CodeInstruction>(instr);

                var bullet = typeof(global::WeaponScript).GetField(
                    nameof(global::WeaponScript.Bullet),
                    BindingFlags.NonPublic | BindingFlags.Instance
                );

                var invoker = typeof(Weapon).GetMethod(
                    nameof(Weapon.InvokeShotFiredSecondary),
                    BindingFlags.NonPublic | BindingFlags.Static
                );

                modified.Insert(EventHookOpCodeIndex, new CodeInstruction(OpCodes.Call, invoker));
                modified.Insert(EventHookOpCodeIndex, new CodeInstruction(OpCodes.Ldloc_1));

                var label = modified[PrevBranchOpCodeIndex].labels[0];
                modified[PrevBranchOpCodeIndex].labels.Clear();
                modified[EventHookOpCodeIndex].labels.Add(label);

                return modified;
            }

            public static void Apply(HarmonyInstance harmony)
            {
                var methodInfo = typeof(global::WeaponScript).GetNestedType(SecondaryFireCoroutineClassName, BindingFlags.NonPublic).GetMethod("MoveNext", BindingFlags.NonPublic | BindingFlags.Instance);
                var transpilerMethod = typeof(SecondaryFire).GetMethod(nameof(Transpiler), BindingFlags.NonPublic | BindingFlags.Static);

                harmony.Patch(methodInfo, transpiler: new HarmonyMethod(transpilerMethod));
            }
        }
    }
}
