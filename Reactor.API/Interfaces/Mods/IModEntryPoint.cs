using Reactor.API.Interfaces.Systems;

namespace Reactor.API.Interfaces.Mods
{
    public interface IModEntryPoint
    {
        void Initialize(IManager manager, string ipcIdentifier);
    }
}
