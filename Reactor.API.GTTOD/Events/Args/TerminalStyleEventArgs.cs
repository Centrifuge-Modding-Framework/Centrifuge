using System;
using UnityEngine;

namespace Reactor.API.GTTOD.Events.Args
{
    public class TerminalStyleEventArgs : EventArgs
    {
        public GUIStyle Style { get; private set; }

        public TerminalStyleEventArgs(GUIStyle style)
        {
            Style = style;
        }
    }
}
