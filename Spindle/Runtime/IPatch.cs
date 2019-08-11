using Mono.Cecil;
using Spindle.Runtime.EventArgs;
using System;

namespace Spindle.Runtime
{
    public interface IPatch
    {
        string Name { get; }
        bool NeedsSource { get; }

        event EventHandler<PatchFailedEventArgs> PatchFailed;
        event EventHandler<PatchSucceededEventArgs> PatchSucceeded;

        void Run(ModuleDefinition moduleDefinition);
        void Run(ModuleDefinition sourceModule, ModuleDefinition targetModule);
    }
}
