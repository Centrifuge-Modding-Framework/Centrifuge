using Reactor.API.DataModel;

namespace Reactor.API.Events
{
    public class ModInitializationEventArgs : System.EventArgs
    {
        public ModInfo Mod { get; }

        public ModInitializationEventArgs(ModInfo mod)
        {
            Mod = mod;
        }
    }
}
