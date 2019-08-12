using Reactor.API.DataModel;
using System.Collections.Generic;

namespace Reactor.API.Interfaces.Systems
{
    public interface IMessenger
    {
        void Send(ModMessage message);
        bool HasHandlerFor(string modId, string messageName);

        List<string> GetRegisteredModIDs();
        List<string> GetRegisteredMessageNamesFor(string modId);
    }
}
