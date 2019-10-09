using Mono.Cecil;
using System.Linq;

namespace Spindle.Runtime
{
    public class PatchHelper
    {
        public static MethodReference ImportBootstrapMethodReference(ModuleDefinition targetModule, ModuleDefinition bootstrapModule)
        {
            var bootstrapType = bootstrapModule.GetType(Resources.CentrifugeBootstrapTypeName);
            var bootstrapInitMethod = bootstrapType.Methods.Single(m => m.Name == Resources.CentrifugeInitMethodName);

            return targetModule.ImportReference(bootstrapInitMethod);
        }
    }
}
