using System;

namespace Centrifuge
{
    public static class EarlyLog
    {
        public static void Info(string message)
        {
            WriteMessage('i', message);
        }

        public static void Warning(string message)
        {
            WriteMessage('*', message);
        }

        public static void Error(string message)
        {
            WriteMessage('!', message);
        }

        public static void Exception(Exception e)
        {
            WriteMessage('e', e.Message);

            if (!string.IsNullOrEmpty(e.StackTrace))
                Console.WriteLine(e.StackTrace);

            if (e.InnerException != null)
            {
                Separator("INNER EXCEPTION FOLLOWS");
                Exception(e.InnerException);
            }
        }

        public static void Separator(string message)
        {
            Console.WriteLine($"-------------- {message} --------------");
        }

        private static void WriteMessage(char symbol, string message)
        {
            Console.WriteLine($"[{symbol}][{DateTime.Now}] {message}");
        }
    }
}
