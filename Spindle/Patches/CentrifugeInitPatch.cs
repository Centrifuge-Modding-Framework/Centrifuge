﻿using Mono.Cecil;
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
                var attributes = MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Assembly;

                var initMethodReference = PatchHelper.ImportBootstrapMethodReference(targetModule, sourceModule);
                var cctor = new MethodDefinition(".cctor", attributes, initMethodReference.ReturnType);
                var ilProcessor = cctor.Body.GetILProcessor();

                var initInstruction = ilProcessor.Create(OpCodes.Call, initMethodReference);
                ilProcessor.Append(initInstruction);
                ilProcessor.Append(ilProcessor.Create(OpCodes.Ret));

                var moduleClass = FindModuleInitializer(targetModule);

                if (moduleClass == null)
                {
                    moduleClass = new TypeDefinition("", "<Module>", TypeAttributes.Class);
                    targetModule.Types.Add(moduleClass);
                }

                moduleClass.Methods.Add(cctor);
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
