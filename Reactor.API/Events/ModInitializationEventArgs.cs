using Reactor.API.DataModel;

namespace Reactor.API.Events
{
    public class ModInitializationEventArgs : System.EventArgs
    {
        public ModInfo Mod { get; }
        public bool IsLast { get; }

        public ModInitializationEventArgs(ModInfo mod, bool isLast)
        {
            Mod = mod;
            IsLast = isLast;
        }
    }
}
