using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Reactor.API.Logging
{
    internal static class ConsolidatedLog
    {
        private static Queue<string> ConsolidatedLogMessageQueue { get; }
        private static StreamWriter StreamWriter { get; set; }
        private static Thread WriterThread { get; set; }

        internal static bool IsRunning { get; private set; }

        static ConsolidatedLog()
        {
            ConsolidatedLogMessageQueue = new Queue<string>();
            ConsolidatedLogMessageQueue.Enqueue("--- NEW LAUNCH ---");
        }

        internal static void Start()
        {
            if (IsRunning) return;
            
            StreamWriter = new StreamWriter(Defaults.ConsolidatedLogFilePath, true) { AutoFlush = true };

            WriterThread = new Thread(WriterThreadHandler);
            WriterThread.Start();

            IsRunning = true;
        }

        internal static void Stop()
        {
            if (!IsRunning) return;
            
            WriterThread.Abort();
            StreamWriter.Dispose();

            IsRunning = false;
        }

        internal static void WriteLine(string text)
        {
            ConsolidatedLogMessageQueue.Enqueue(text);
        }

        private static void WriterThreadHandler()
        {
            while (true)
            {
                Thread.Sleep(500);

                while (ConsolidatedLogMessageQueue.Count != 0)
                {
                    StreamWriter.WriteLine(ConsolidatedLogMessageQueue.Dequeue());
                }
            }
        }
    }
}
