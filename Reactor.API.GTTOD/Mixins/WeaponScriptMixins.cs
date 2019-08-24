﻿using Harmony;
using Reactor.API.GTTOD.Events;
using Reactor.API.GTTOD.Events.Args;

namespace Reactor.API.GTTOD.Mixins
{
    [HarmonyPatch(typeof(WeaponScript), "Awake")]
    internal class WeaponScriptAwakeMixins
    {
        public static bool Prefix(WeaponScript __instance)
        {
            var eventArgs = new MethodPreviewEventArgs<WeaponScript>(__instance);
            Weapon.InvokePreviewAwake(eventArgs);

            return !eventArgs.Cancel;
        }

        public static void Postfix(WeaponScript __instance)
        {
            var eventArgs = new TypeInstanceEventArgs<WeaponScript>(__instance);
            Weapon.InvokeAwakeComplete(eventArgs);
        }
    }
}
