using Mono.Cecil;
using Spindle.Enums;

using System;

namespace Spindle.IO
{
    public class ModuleLoader
    {
        public static ModuleDefinition LoadGameModule(string gameAssemblyFileName)
        {
            try
            {
                ColoredOutput.WriteInformation("Loading TARGET module...");
                return ModuleDefinition.ReadModule(gameAssemblyFileName, new ReaderParameters() { ReadWrite = true, InMemory = true });
            }
            catch (Exception e)
            {
                ErrorHandler.TerminateWithError($"Couldn't load TARGET module definition. Exception details:\n{e}", TerminationReason.TargetModuleLoadFailed);
            }
            return null;
        }

        public static ModuleDefinition LoadBootstrapModule(string bootstrapAssemblyFilename)
        {
            try
            {
                ColoredOutput.WriteInformation("Loading BOOTSTRAP module...");
                return ModuleDefinition.ReadModule(bootstrapAssemblyFilename);
            }
            catch (Exception e)
            {
                ErrorHandler.TerminateWithError($"Could't load BOOTSTRAP module definition. Exception details:\n{e}", TerminationReason.BootstrapModuleLoadFailed);
            }
            return null;
        }
    }
}
