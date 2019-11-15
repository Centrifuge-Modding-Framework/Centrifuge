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
        private Log Log => LogManager.GetForInternalAssembly();
        private Dictionary<string, Dictionary<string, List<MethodInfo>>> MessageHandlers { get; }

        public Messenger()
        {
            MessageHandlers = new Dictionary<string, Dictionary<string, List<MethodInfo>>>();
        }

        public void Send(ModMessage message)
        {
            if (HasHandlerFor(message.TargetModID, message.Name))
            {
                IssueTargetedBroadcast(message);
            }
        }

        public void Broadcast(ModMessage message)
        {
            if (HasHandlerFor("*", message.Name))
            {
                IssueBroadcast(message);
            }
        }

        public List<string> GetRegisteredModIDs()
        {
            return new List<string>(MessageHandlers.Keys);
        }

        public List<string> GetRegisteredMessageNamesFor(string modId)
        {
            var ret = new List<string>();

            if (MessageHandlers.ContainsKey(modId))
            {
                ret.AddRange(MessageHandlers[modId].Keys);
            }

            return ret;
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
                    Log.Error($"An exception occured in the targeted handler '{handler.Name}' from mod '{message.TargetModID}'");
                    Log.Exception(e);
                }
            }
        }

        private void IssueBroadcast(ModMessage message)
        {
            foreach (var handler in MessageHandlers["*"][message.Name])
            {
                try
                {
                    handler.Invoke(null, new object[] { message });
                }
                catch (Exception e)
                {
                    Log.Error($"An exception occured in the broadcast handler '{handler.Name}'.");
                    Log.Exception(e);
                }
            }
        }
    }
}
