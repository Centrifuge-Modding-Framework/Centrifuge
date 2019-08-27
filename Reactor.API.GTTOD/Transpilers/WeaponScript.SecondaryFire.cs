using Harmony;
using Reactor.API.GTTOD.Events;
using Reactor.API.GTTOD.Infrastructure;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Reactor.API.GTTOD.Transpilers
{
    internal static partial class WeaponScript
    {
        private class SecondaryFire : GameCodeTranspiler
        {
            private const int EventHookOpCodeIndex = 129;
            private const string SecondaryFireCoroutineClassName = "<SecondaryFire>d__140";

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

                return modified;
            }

            public override void Apply(HarmonyInstance harmony)
            {
                var methodInfo = typeof(global::WeaponScript).GetNestedType(SecondaryFireCoroutineClassName, BindingFlags.NonPublic).GetMethod("MoveNext", BindingFlags.NonPublic | BindingFlags.Instance);
                var transpilerMethod = typeof(SecondaryFire).GetMethod(nameof(Transpiler), BindingFlags.NonPublic | BindingFlags.Static);

                harmony.Patch(methodInfo, transpiler: new HarmonyMethod(transpilerMethod));
            }
        }
    }
}
