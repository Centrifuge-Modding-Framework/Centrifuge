using Harmony;

namespace Reactor.API.GameSupport.Patching
{
    public abstract class CodeTranspiler
    {
        public abstract void Apply(HarmonyInstance harmonyInstance);
    }
}
