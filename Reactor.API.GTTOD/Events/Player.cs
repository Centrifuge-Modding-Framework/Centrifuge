using System;

namespace Reactor.API.GTTOD.Events
{
    public static class Player
    {
        public static event EventHandler Died;

        internal static void InvokeDied()
        {
            Died?.Invoke(null, EventArgs.Empty);
        }
    }
}
