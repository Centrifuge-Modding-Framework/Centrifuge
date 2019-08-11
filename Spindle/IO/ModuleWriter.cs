using Mono.Cecil;
using Spindle.Enums;
using System;

namespace Spindle.IO
{
    public static class ModuleWriter
    {
        public static void SavePatchedFile(ModuleDefinition module, string fileName, bool disposeModule)
        {
            try
            {
                module.Write(fileName);

                if (disposeModule)
                    module.Dispose();
            }
            catch (AssemblyResolutionException)
            {
                ErrorHandler.TerminateWithError("Can't find the required dependencies. Make sure you run Prism inside the 'Managed' directory.", TerminationReason.RequiredDependenciesMissing);
            }
            catch (Exception e)
            {
                ErrorHandler.TerminateWithError($"Can't write back the modified assembly. Is it in use and/or you don't have write rights?\n{e}", TerminationReason.AssemblySaveFailed);
            }
        }
    }
}
