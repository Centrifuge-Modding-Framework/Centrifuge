using Harmony;

namespace Reactor.API.GameSupport.Patching
{
    public abstract class GameCodeTranspiler
    {
        public abstract void Apply(HarmonyInstance harmonyInstance);
    }
}
