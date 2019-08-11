using Spindle.Enums;
using Spindle.IO;
using System;

namespace Spindle
{
    internal class ErrorHandler
    {
        public static void TerminateWithError(string message, TerminationReason reason = 0)
        {
            ColoredOutput.WriteError(message);
            Environment.Exit((int)reason);
        }
    }
}
