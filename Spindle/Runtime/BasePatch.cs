using Mono.Cecil;
using Spindle.Runtime.EventArgs;
using System;

namespace Spindle.Runtime
{
    public class BasePatch : IPatch
    {
        public virtual string Name => "BasePatch";
        public virtual bool NeedsSource => false;

        public event EventHandler<PatchFailedEventArgs> PatchFailed;
        public event EventHandler<PatchSucceededEventArgs> PatchSucceeded;

        public virtual void Run(ModuleDefinition moduleDefinition)
        {
            var eventArgs = new PatchFailedEventArgs(Name, new Exception("This patch requires both source and target modules."));
            PatchFailed?.Invoke(this, eventArgs);
        }

        public virtual void Run(ModuleDefinition sourceModule, ModuleDefinition targetModule)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnPatchSucceeded(object sender, PatchSucceededEventArgs e)
        {
            PatchSucceeded?.Invoke(sender, e);
        }

        protected virtual void OnPatchFailed(object sender, PatchFailedEventArgs e)
        {
            PatchFailed?.Invoke(sender, e);
        }
    }
}
