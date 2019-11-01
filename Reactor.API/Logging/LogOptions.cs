using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reactor.API.Logging
{
    public class LogOptions
    {
        public bool WriteToConsole { get; set; }
        public bool ColorizeLines { get; set; }

        public LogOptions()
        {
            WriteToConsole = true;
            ColorizeLines = true;
        }
    }
}
