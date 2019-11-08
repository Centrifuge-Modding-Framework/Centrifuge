using Centrifuge.UnityInterop.Bridges;
using Reactor.API.Extensions;
using System;
using System.IO;
using System.Reflection;

namespace Reactor.API.Logging
{
    public class Log
    {
        private string RootDirectory { get; }
        private string FileName { get; }

        private string FilePath => Path.Combine(
            Path.Combine(RootDirectory, Defaults.PrivateLogDirectory), 
            FileName
        );

        public LogOptions Options { get; }

        public Log(string fileName)
        {
            Options = new LogOptions();

            RootDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            FileName = $"{fileName}.txt";

            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

            if (File.Exists(FilePath))
                File.Delete(FilePath);

        }

        public Log(string fileName, LogOptions options)
            : this(fileName)
        {
            Options = options;
        }

        public void Error(string message)
        {
            if (!Options.Toggles.HasFlag(LogToggles.Error))
                return;

            var msg = $"[!][{DateTime.Now}] {message}";

            ColorizeIfPossible(
                () => WriteLine(message),
                ConsoleColor.Red
            );
        }

        public void Warning(string message)
        {
            if (!Options.Toggles.HasFlag(LogToggles.Warning))
                return;

            var msg = $"[*][{DateTime.Now}] {message}";

            ColorizeIfPossible(
                () => WriteLine(message),
                ConsoleColor.Yellow
            );
        }

        public void Success(string message)
        {
            if (!Options.Toggles.HasFlag(LogToggles.Error))
                return;

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
            if (Options.UseConsolidatedLogFile)
                ConsolidatedLog.WriteLine(text);

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
