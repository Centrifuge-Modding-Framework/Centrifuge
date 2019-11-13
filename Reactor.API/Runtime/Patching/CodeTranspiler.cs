using Harmony;

namespace Reactor.API.Runtime.Patching
{
    public abstract class CodeTranspiler
    {
        public abstract void Apply(HarmonyInstance harmonyInstance);
    }
}
