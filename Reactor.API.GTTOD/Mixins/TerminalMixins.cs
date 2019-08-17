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

        public static void Postfix()
        {
            Internal.Terminal.OnInitFinished();
        }
    }

    [HarmonyPatch(typeof(Terminal), "SetupInput")]
    internal class TerminalSetupInputMixin
    {
        public static void Postfix(Terminal __instance)
        {
            Internal.Terminal.OnInputStyleSet(__instance.input_style);
        }
    }

    [HarmonyPatch(typeof(Terminal), "SetupWindow")]
    internal class TerminalSetupWindowMixin
    {
        public static void Postfix(Terminal __instance)
        {
            Internal.Terminal.OnWindowStyleSet(__instance.window_style);
        }
    }

    [HarmonyPatch(typeof(Terminal), "SetupLabels")]
    internal class TerminalSetupLabelsMixin
    {
        public static void Postfix(Terminal __instance)
        {
            Internal.Terminal.OnLabelStyleSet(__instance.label_style);
        }
    }

    [HarmonyPatch(typeof(Terminal), "ToggleState")]
    internal class TerminalToggleStateMixin
    {
        public static void Postfix(Terminal __instance)
        {
            if (__instance.state == TerminalState.Close)
            {
                Internal.Terminal.OnClosed(__instance);
            }
            else
            {
                // FIXME: might trigger second time
                //        if user expands from small 
                //        size to large size

                Internal.Terminal.OnOpened(__instance);
            }
        }
    }
}
