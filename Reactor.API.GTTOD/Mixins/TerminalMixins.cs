using CommandTerminal;
using Harmony;
using UnityEngine;

namespace Reactor.API.GTTOD.Mixins
{
    [HarmonyPatch(typeof(Terminal), "OnEnable")]
    internal class TerminalOnEnableMixin
    {
        public static bool Prefix(Terminal __instance)
        {
            if (__instance.gameObject.name == "Game")
                return false;

            return true;
        }
    }

    [HarmonyPatch(typeof(Terminal), "Start")]
    internal class TerminalStartMixin
    {
        public static bool Prefix(Terminal __instance)
        {
            if (__instance.gameObject.name == "Game")
            {
                GameObject.Destroy(__instance);
                return false;
            }

            return true;
        }
    }
}
