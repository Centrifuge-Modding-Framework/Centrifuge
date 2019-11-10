using System;

namespace Spindle.IO
{
    public class ColoredOutput
    {
        public static void WriteError(string message)
        {
            WriteColoredText($"[!] {message}", ConsoleColor.Red);
        }

        public static void WriteSuccess(string message)
        {
            WriteColoredText($"[+] {message}", ConsoleColor.Green);
        }

        public static void WriteInformation(string message)
        {
            WriteColoredText($"[i] {message}", ConsoleColor.White);
        }

        private static void WriteColoredText(string message, ConsoleColor consoleColor)
        {
            if (Platform.IsMono() || Platform.IsUnix())
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.ForegroundColor = consoleColor;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }
    }
}
