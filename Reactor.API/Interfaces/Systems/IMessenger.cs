using Reactor.API.DataModel;

namespace Reactor.API.Interfaces.Systems
{
    public interface IMessenger
    {
        void Send(Message message);
    }
}
