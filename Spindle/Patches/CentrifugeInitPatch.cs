using Mono.Cecil;
using Mono.Cecil.Cil;
using Spindle.Runtime;
using Spindle.Runtime.EventArgs;
using System;

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
                var targetMethod = PatchHelper.FindInitializationMethodDefinition(targetModule);
                var ilProcessor = targetMethod.Body.GetILProcessor();

                var initMethodReference = PatchHelper.ImportBootstrapMethodReference(targetModule, sourceModule);
                var initializationInstruction = ilProcessor.Create(OpCodes.Call, initMethodReference);

                var lastAwakeInstruction = ilProcessor.Body.Instructions[ilProcessor.Body.Instructions.Count - 1];

                if (lastAwakeInstruction.OpCode == OpCodes.Call)
                {
                    var eventArgs = new PatchFailedEventArgs(Name, new Exception("This patch has already been applied."));
                    OnPatchFailed(this, eventArgs);

                    return;
                }

                ilProcessor.InsertBefore(lastAwakeInstruction, initializationInstruction);
                OnPatchSucceeded(this, new PatchSucceededEventArgs(Name));
            }
            catch (Exception ex)
            {
                var eventArgs = new PatchFailedEventArgs(Name, ex);
                OnPatchFailed(this, eventArgs);
            }
        }
    }
}
