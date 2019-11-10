using Mono.Cecil;
using System.Linq;

namespace Spindle.Runtime
{
    public class PatchHelper
    {
        public static MethodReference ImportBootstrapMethodReference(ModuleDefinition targetModule, ModuleDefinition bootstrapModule)
        {
            var bootstrapType = bootstrapModule.GetType(Resources.CentrifugeBootstrapTypeName);
            var bootstrapInitMethod = FindBootstrapInitMethod(bootstrapType);

            return targetModule.ImportReference(bootstrapInitMethod);
        }

        private static MethodDefinition FindBootstrapInitMethod(TypeDefinition bootstrapType)
        {
            foreach (var method in bootstrapType.Methods)
            {
                if (method.Name == Resources.CentrifugeInitMethodName)
                    return method;
            }

            return null;
        }
    }
}
