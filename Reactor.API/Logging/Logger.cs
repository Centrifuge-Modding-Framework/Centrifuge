using System;
using System.IO;
using System.Reflection;

namespace Reactor.API.Logging
{
    public class Logger
    {
        public ConsoleColor ErrorColor { get; set; } = ConsoleColor.Red;
        public ConsoleColor WarningColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor SuccessColor { get; set; } = ConsoleColor.Green;
        public ConsoleColor InfoColor { get; set; } = ConsoleColor.White;
        public ConsoleColor ExceptionColor { get; set; } = ConsoleColor.Magenta;

        public bool WriteToConsole { get; set; } = true;
        public bool ColorizeLines { get; set; }

        private string RootDirectory { get; }
        private string FileName { get; }

        private string FilePath => Path.Combine(Path.Combine(RootDirectory, Defaults.PrivateLogDirectory), FileName);

        public Logger(string fileName)
        {
            ColorizeLines = true;

            RootDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            FileName = $"{fileName}.log";

            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }

        public void Error(string message)
        {
            if (ColorizeLines)
                Console.ForegroundColor = ErrorColor;

            WriteLine($"[!][{DateTime.Now}] {message}");

            if (ColorizeLines)
                Console.ResetColor();
        }

        public void Warning(string message)
        {
            if (ColorizeLines)
                Console.ForegroundColor = WarningColor;

            WriteLine($"[*][{DateTime.Now}] {message}");

            if (ColorizeLines)
                Console.ResetColor();
        }

        public void Success(string message)
        {
            if (ColorizeLines)
                Console.ForegroundColor = SuccessColor;

            WriteLine($"[+][{DateTime.Now}] {message}");

            if (ColorizeLines)
                Console.ResetColor();
        }

        public void Info(string message, bool noNewLine = false)
        {
            if (ColorizeLines)
                Console.ForegroundColor = InfoColor;

            var msg = $"[i][{DateTime.Now}] {message}";

            if (noNewLine)
                Write(msg);
            else
                WriteLine(msg);

            if (ColorizeLines)
                Console.ResetColor();
        }

        public void Exception(Exception e)
        {
            if (ColorizeLines)
                Console.ForegroundColor = ExceptionColor;

            WriteLine($"[e][{DateTime.Now}] {e.Message}");

            if (ColorizeLines)
                Console.ResetColor();

            WriteLine($"   Target site: {e.TargetSite}");
            WriteLine("   Stack trace:");

            foreach (var s in e.StackTrace.Split(Environment.NewLine.ToCharArray()))
                WriteLine($"      {s}");

            if (e.InnerException != null)
            {
                WriteLine("--- Inner exception below ---");
                Exception(e.InnerException);
            }
        }

        public void ExceptionSilent(Exception e)
        {
            WriteLineSilent($"[e][{DateTime.Now}] {e.Message}");
            WriteLineSilent($"   Target site: {e.TargetSite}");

            WriteLineSilent("   Stack trace:");

            foreach (var s in e.StackTrace.Split(Environment.NewLine.ToCharArray()))
                WriteLineSilent($"      {s}");

            if (e.InnerException != null)
            {
                WriteLineSilent("--- Inner exception below ---");
                ExceptionSilent(e.InnerException);
            }
        }

        public void WriteLine(string text)
        {
            using (var sw = new StreamWriter(FilePath, true))
                sw.WriteLine(text);

            if (WriteToConsole)
                Console.WriteLine(text);
        }

        private void WriteLineSilent(string text)
        {
            using (var sw = new StreamWriter(FilePath, true))
                sw.WriteLine(text);
        }

        private void Write(string text)
        {
            using (var sw = new StreamWriter(FilePath, true))
                sw.Write(text);

            if (WriteToConsole)
                Console.Write(text);
        }
    }
}
