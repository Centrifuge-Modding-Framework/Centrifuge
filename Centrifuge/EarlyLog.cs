using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Centrifuge
{
    internal static class EarlyLog
    {
        private static StreamWriter LogFileWriter { get; }

        static EarlyLog()
        {
            var dirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var logPath = Path.Combine(dirPath, "Centrifuge.log");

            LogFileWriter = new StreamWriter(logPath) { AutoFlush = true };
        }

        public static void Info(string message)
        {
            WriteMessage("INF", message);
        }

        public static void Warning(string message)
        {
            WriteMessage("WRN", message);
        }

        public static void Error(string message)
        {
            WriteMessage("ERR", message);
        }

        public static void Exception(Exception e)
        {
            var sb = new StringBuilder();

            sb.AppendLine(e.Message);

            if (!string.IsNullOrEmpty(e.StackTrace))
                sb.AppendLine(e.StackTrace);

            if (e.InnerException != null)
            {
                sb.AppendLine(" --- INNER EXCEPTION FOLLOWS --- ");
                sb.AppendLine(e.InnerException.ToString());

                if (e.InnerException is ReflectionTypeLoadException rtle)
                {
                    sb.AppendLine();
                    foreach (var le in rtle.LoaderExceptions)
                    {
                        sb.AppendLine($"  {le.Message}");
                    }
                }
            }

            WriteMessage("EXC", sb.ToString());
        }

        internal static void CleanUp()
        {
            LogFileWriter.Dispose();
        }

        private static void WriteMessage(string descriptor, string message)
        {
            var msgFormat = $"[{DateTime.Now.ToString("HH:mm:ss")} {descriptor}] [{nameof(Bootstrap)}] {message}";

            LogFileWriter.WriteLine(msgFormat);
            Console.WriteLine(msgFormat);
        }
    }
}
