using Harmony;

namespace Reactor.API.GTTOD.Infrastructure
{
    public abstract class GameCodeTranspiler
    {
        public abstract void Apply(HarmonyInstance harmony);
    }
}
