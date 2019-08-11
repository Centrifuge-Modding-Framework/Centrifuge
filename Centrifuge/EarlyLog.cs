using System;

namespace Centrifuge
{
    internal static class EarlyLog
    {
        public static void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            WriteMessage('i', message);
            Console.ResetColor();
        }

        public static void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteMessage('*', message);
            Console.ResetColor();
        }

        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteMessage('!', message);
            Console.ResetColor();
        }

        public static void Exception(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            WriteMessage('e', e.Message);

            if (!string.IsNullOrEmpty(e.StackTrace))
                Console.WriteLine(e.StackTrace);

            if (e.InnerException != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("-------------- INNER EXCEPTION FOLLOWS --------------");
                Console.ForegroundColor = ConsoleColor.Magenta;

                Exception(e.InnerException);
            }

            Console.ResetColor();
        }

        private static void WriteMessage(char symbol, string message)
        {
            Console.WriteLine($"[{symbol}][{DateTime.Now}] {message}");
        }
    }
}
