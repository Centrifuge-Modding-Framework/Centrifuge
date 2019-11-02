using Centrifuge.UnityInterop.Bridges;
using System;
using System.IO;
using System.Reflection;

namespace Reactor.API.Logging
{
    public class Log
    {
        public LogOptions Options { get; }

        private string RootDirectory { get; }
        private string FileName { get; }

        private string FilePath => Path.Combine(Path.Combine(RootDirectory, Defaults.PrivateLogDirectory), FileName);

        /// <summary>
        /// Creates a logger integrated with Centrifuge.
        /// </summary>
        /// <param name="fileName">Log file name without an extension.</param>
        public Log(string fileName)
        {
            RootDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            FileName = $"{fileName}.txt";

            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

            if (File.Exists(FilePath))
                File.Delete(FilePath);

            Options = new LogOptions();
        }
        
        /// <summary>
        /// Creates a logger integrated with Centrifuge, allows you to provide alternative logging configuration.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="options"></param>
        public Log(string fileName, LogOptions options)
            : this(fileName)
        {
            Options = options;
        }

        public void Error(string message)
        {
            var msg = $"[!][{DateTime.Now}] {message}";

            ColorizeIfPossible(
                () => WriteLine(message),
                ConsoleColor.Red
            );
        }

        public void Warning(string message)
        {
            var msg = $"[*][{DateTime.Now}] {message}";

            ColorizeIfPossible(
                () => WriteLine(message),
                ConsoleColor.Yellow
            );
        }

        public void Success(string message)
        {
            var msg = $"[+][{DateTime.Now}] {message}";

            ColorizeIfPossible(
                () => WriteLine(message),
                ConsoleColor.Green
            );
        }

        public void Info(string message)
        {
            var msg = $"[i][{DateTime.Now}] {message}";

            ColorizeIfPossible(
                () => WriteLine(msg),
                ConsoleColor.White
            );
        }

        public void Exception(Exception e, bool silent = false)
        {
            WriteLine($"[e][{DateTime.Now}] {e.Message}", silent);

            if (e.TargetSite != null)
            {
                WriteLine($"   What threw: {e.TargetSite.Name} in {e.TargetSite.DeclaringType.Name}", silent);
            }

            if (!string.IsNullOrEmpty(e.StackTrace))
            {
                WriteLine("   Stack trace:", silent);
                foreach (var s in e.StackTrace.Split(Environment.NewLine.ToCharArray()))
                    WriteLine($"      {s}", silent);
            }

            if (e.InnerException != null)
            {
                WriteLine("--- Inner exception below ---", silent);
                Exception(e.InnerException, silent);
            }
        }

        public void TypeResolverFailure(ReflectionTypeLoadException rtle)
        {
            Exception(rtle);

            foreach (var le in rtle.LoaderExceptions)
            {
                ColorizeIfPossible(
                    () => WriteLine(le.ToString()),
                    ConsoleColor.Cyan
                );
            }
        }

        public void WriteLine(string text, bool suppressConsole = false)
        {
            using (var sw = new StreamWriter(FilePath, true))
                sw.WriteLine(text);

            if (Options.WriteToConsole && !suppressConsole)
                Console.WriteLine(text);
        }

        private void ColorizeIfPossible(Action action, ConsoleColor color)
        {
            var consoleSupportsColor = ConsoleBridge.IsConsoleForegroundPropertyPresent();

            if (consoleSupportsColor && Options.ColorizeLines)
                Console.ForegroundColor = color;

            action.Invoke();

            if (consoleSupportsColor && Options.ColorizeLines)
                Console.ResetColor();
        }
    }
}
