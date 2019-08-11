using Reactor.API.DataModel;

namespace Reactor.API.Interfaces.Systems
{
    public interface IMessenger
    {
        void Send(ModMessage message);
        bool HasHandlerFor(string modId, string messageName);
    }
}
