using System;

namespace Centrifuge.UnityInterop.Bridges
{
    public static class ConsoleBridge
    {
        public static Type ConsoleType => typeof(Console);

        public static bool IsConsoleForegroundPropertyPresent()
        {
            return ConsoleType.GetProperty("ForegroundColor") != null;
        }
    }
}
