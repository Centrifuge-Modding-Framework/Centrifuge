using System;

namespace Reactor.API.GTTOD.Events.Args
{
    public class ApiEventArgsBase<T> : EventArgs
    {
        public T Instance { get; }

        public ApiEventArgsBase(T instance)
        {
            Instance = instance;
        }
    }
}
