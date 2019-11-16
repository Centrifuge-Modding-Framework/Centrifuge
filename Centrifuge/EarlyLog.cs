using System;
using System.Reflection;

namespace Centrifuge
{
    public static class EarlyLog
    {
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
            WriteMessage("EXC", e.Message);

            if (!string.IsNullOrEmpty(e.StackTrace))
                Console.WriteLine(e.StackTrace);

            if (e.InnerException != null)
            {
                Separator("INNER EXCEPTION FOLLOWS");
                Exception(e.InnerException);

                if (e.InnerException is ReflectionTypeLoadException rtle)
                {
                    Console.WriteLine();

                    foreach (var le in rtle.LoaderExceptions)
                    {
                        Console.WriteLine($"  {le.Message}");
                    }
                }
            }
        }

        public static void Separator(string message)
        {
            Console.WriteLine($"--------------{message}--------------");
        }

        private static void WriteMessage(string descriptor, string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")} {descriptor}] [{nameof(Bootstrap)}] {message}");
        }
    }
}
