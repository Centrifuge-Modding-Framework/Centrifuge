using Harmony;

namespace Reactor.API.GTTOD.Mixins
{
    [HarmonyPatch(typeof(MenuScript), nameof(MenuScript.OpenMenu))]
    internal class OpenMenuMixin
    {
        public static void Postfix()
            => Events.Game.InvokePauseMenuOpened();
    }

    [HarmonyPatch(typeof(MenuScript), nameof(MenuScript.CloseMenu))]
    internal class CloseMenuMixin
    {
        public static void Postfix()
            => Events.Game.InvokePauseMenuClosed();
    }
}
