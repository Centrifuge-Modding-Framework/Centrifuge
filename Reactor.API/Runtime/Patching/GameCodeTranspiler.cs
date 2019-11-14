using Harmony;

namespace Reactor.API.Runtime.Patching
{
    public abstract class GameCodeTranspiler
    {
        public abstract void Apply(HarmonyInstance harmonyInstance);
    }
}
