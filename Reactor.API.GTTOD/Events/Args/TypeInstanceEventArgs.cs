using System;
using System.Collections.Generic;

namespace Reactor.API.GTTOD.Events.Args
{
    public class TypeInstanceEventArgs<T> : EventArgs
    {
        public T Instance { get; }
        public Dictionary<string, object> AdditionalData { get; }

        public TypeInstanceEventArgs(T instance)
        {
            Instance = instance;
            AdditionalData = new Dictionary<string, object>();
        }
    }
}
