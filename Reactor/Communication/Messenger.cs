using Reactor.API;
using Reactor.API.DataModel;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Reactor.Communication
{
    internal class Messenger : IMessenger
    {
        private Logger Log { get; }

        private Dictionary<string, Dictionary<string, List<MethodInfo>>> MessageHandlers { get; }

        public Messenger()
        {
            Log = new Logger(Defaults.MessengerLogFileName);
            MessageHandlers = new Dictionary<string, Dictionary<string, List<MethodInfo>>>();
        }

        public void Send(ModMessage message)
        {
            if (HasHandlerFor(message.TargetModID, message.Name))
            {
                IssueTargetedBroadcast(message);
            }
        }

        public bool HasHandlerFor(string modId, string messageName)
        {
            if (!MessageHandlers.ContainsKey(modId))
            {
                return false;
            }

            if (!MessageHandlers[modId].ContainsKey(messageName))
            {
                return false;
            }

            return true;
        }

        internal void RegisterHandlerFor(string modId, string messageName, MethodInfo messageHandler)
        {
            EnsureCollectionExists(modId, messageName);
            MessageHandlers[modId][messageName].Add(messageHandler);
        }

        private void EnsureCollectionExists(string modId, string messageName)
        {
            if (!MessageHandlers.ContainsKey(modId))
                MessageHandlers.Add(modId, new Dictionary<string, List<MethodInfo>>());

            if (!MessageHandlers[modId].ContainsKey(messageName))
                MessageHandlers[modId].Add(messageName, new List<MethodInfo>());
        }

        private void IssueTargetedBroadcast(ModMessage message)
        {
            foreach (var handler in MessageHandlers[message.TargetModID][message.Name])
            {
                try
                {
                    handler.Invoke(null, new object[] { message });
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
            }
        }
    }
}
