using Mono.Cecil;
using Mono.Cecil.Cil;
using Spindle.Runtime;
using Spindle.Runtime.EventArgs;
using System;

namespace Spindle.Patches
{
    public class CentrifugeUpdatePatch : BasePatch
    {
        public override string Name => "CentrifugeFrameUpdate";
        public override bool NeedsSource => true;

        public override void Run(ModuleDefinition sourceModule, ModuleDefinition targetModule)
        {
            try
            {
                var targetMethod = PatchHelper.FindFrameUpdateMethodDefinition(targetModule);
                var ilProcessor = targetMethod.Body.GetILProcessor();

                var updateMethodReference = PatchHelper.ImportUpdateMethodReference(targetModule, sourceModule);
                var updateInstruction = ilProcessor.Create(OpCodes.Call, updateMethodReference);

                var returnInstruction = ilProcessor.Body.Instructions[ilProcessor.Body.Instructions.Count - 1];

                // naive check, might have to fix this --if-- when andrew 
                // decides to fuck with awake for whatever reason lol
                if (returnInstruction.OpCode == OpCodes.Call)
                {
                    var eventArgs = new PatchFailedEventArgs(Name, new Exception("This patch has already been applied."));
                    OnPatchFailed(this, eventArgs);
                }

                ilProcessor.InsertBefore(returnInstruction, updateInstruction);
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
