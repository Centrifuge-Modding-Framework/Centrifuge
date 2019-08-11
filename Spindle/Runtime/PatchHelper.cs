using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace Spindle.Runtime
{
    public class PatchHelper
    {
        public static MethodDefinition FindInitializationMethodDefinition(ModuleDefinition targetModule)
        {
            var targetType = targetModule.GetType(Resources.InitializationPointTypeName);
            var methodDef = targetType.Methods.SingleOrDefault(m => m.Name == Resources.InitializationPointMethodName);

            if (methodDef == null)
            {
                methodDef = new MethodDefinition("Awake", MethodAttributes.Public, targetModule.ImportReference(typeof(void)));

                var ilProcessor = methodDef.Body.GetILProcessor();
                ilProcessor.Append(ilProcessor.Create(OpCodes.Ret));
                targetType.Methods.Add(methodDef);

                // needs to be done like this, because for some reason
                // returning the methoddef itself does jack shit
                return targetType.Methods.Single(m => m.Name == Resources.InitializationPointMethodName);
            }

            return methodDef;
        }

        public static MethodDefinition FindFrameUpdateMethodDefinition(ModuleDefinition targetModule)
        {
            var targetType = targetModule.GetType(Resources.FrameUpdatePointTypeName);
            var methodDef = targetType.Methods.SingleOrDefault(m => m.Name == Resources.FrameUpdatePointMethodName);

            if (methodDef == null)
            {
                methodDef = new MethodDefinition("Update", MethodAttributes.Public, targetModule.ImportReference(typeof(void)));

                var ilProcessor = methodDef.Body.GetILProcessor();
                ilProcessor.Append(ilProcessor.Create(OpCodes.Ret));
                targetType.Methods.Add(methodDef);

                return targetType.Methods.Single(m => m.Name == Resources.FrameUpdatePointMethodName);
            }

            return methodDef;
        }

        public static MethodReference ImportBootstrapMethodReference(ModuleDefinition targetModule, ModuleDefinition bootstrapModule)
        {
            var bootstrapType = bootstrapModule.GetType(Resources.CentrifugeBootstrapTypeName);
            var bootstrapInitMethod = bootstrapType.Methods.Single(m => m.Name == Resources.CentrifugeInitMethodName);

            return targetModule.ImportReference(bootstrapInitMethod);
        }

        public static MethodReference ImportUpdateMethodReference(ModuleDefinition targetModule, ModuleDefinition bootstrapModule)
        {
            var bootstrapType = bootstrapModule.GetType(Resources.CentrifugeBootstrapTypeName);
            var bootstrapUpdateMethod = bootstrapType.Methods.Single(m => m.Name == Resources.CentrifugeFrameUpdateMethodName);

            return targetModule.ImportReference(bootstrapUpdateMethod);
        }
    }
}
