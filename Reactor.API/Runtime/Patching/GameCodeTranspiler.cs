using HarmonyLib;

namespace Reactor.API.Runtime.Patching
{
    public abstract class GameCodeTranspiler
    {
        public abstract void Apply(Harmony harmony);
    }
}
