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
                Console.WriteLine("-------------- INNER EXCEPTION FOLLOWS --------------");
                Exception(e.InnerException);
            }
        }

        private static void WriteMessage(char symbol, string message)
        {
            Console.WriteLine($"[{symbol}][{DateTime.Now}] {message}");
        }
    }
}
