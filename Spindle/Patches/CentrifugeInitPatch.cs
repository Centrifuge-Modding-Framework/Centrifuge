using Mono.Cecil;
using Mono.Cecil.Cil;
using Spindle.Runtime;
using Spindle.Runtime.EventArgs;
using System;
using System.Linq;

namespace Spindle.Patches
{
    public class CentrifugeInitPatch : BasePatch
    {
        public override string Name => "CentrifugeInit";
        public override bool NeedsSource => true;

        public override void Run(ModuleDefinition sourceModule, ModuleDefinition targetModule)
        {
            try
            {
                var moduleClass = FindModuleInitializer(targetModule);
                if (moduleClass == null)
                {
                    moduleClass = new TypeDefinition("", "<Module>", TypeAttributes.Class);
                    targetModule.Types.Add(moduleClass);
                }

                var initMethodReference = PatchHelper.ImportBootstrapMethodReference(targetModule, sourceModule);
                var cctor = moduleClass.Methods.FirstOrDefault(m => m.Name == ".cctor");

                if (cctor == null)
                {
                    cctor = new MethodDefinition(
                        ".cctor", 
                        MethodAttributes.Static | 
                        MethodAttributes.SpecialName | 
                        MethodAttributes.RTSpecialName | 
                        MethodAttributes.Assembly, 
                        targetModule.ImportReference(typeof(void))
                    );
                    moduleClass.Methods.Add(cctor);
                }

                var ilProcessor = cctor.Body.GetILProcessor();

                var callInstruction = Instruction.Create(OpCodes.Call, initMethodReference);

                if (cctor.Body.Instructions.Count > 0)
                {
                    if (cctor.Body.Instructions.FirstOrDefault(
                        i => i.OpCode == OpCodes.Call && 
                        i.Operand is MethodReference mr && 
                        mr.FullName == initMethodReference.FullName) == null)
                    {
                        ilProcessor.InsertBefore(cctor.Body.Instructions[0], callInstruction);
                    }
                }
                else
                {
                    ilProcessor.Append(callInstruction);
                    ilProcessor.Emit(OpCodes.Ret);
                }

                OnPatchSucceeded(this, new PatchSucceededEventArgs(Name));
            }
            catch (Exception ex)
            {
                var eventArgs = new PatchFailedEventArgs(Name, ex);
                OnPatchFailed(this, eventArgs);
            }
        }

        private static TypeDefinition FindModuleInitializer(ModuleDefinition targetModule)
        {
            foreach (var type in targetModule.Types)
            {
                if (type.Name == "<Module>")
                    return type;
            }

            return null;
        }
    }
}
