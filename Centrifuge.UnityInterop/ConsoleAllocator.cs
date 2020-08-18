using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Centrifuge.UnityInterop
{
    public static class ConsoleAllocator
    {
        private const int StdOutputHandle = -11;
        private const uint EnableVirtualTerminalProcessing = 0x4;
        private const uint DisableNewlineAutoReturn = 0x8;

        private static StreamWriter _outputWriter;
        private static TextWriter _originalStream;

        private static bool _allocated;

        public static bool FancyColorsEnabled { get; set; } = true;
        public static bool IsRedirecting => Console.Out == _outputWriter;
        
        public static void Redirect()
        {
            if (!_allocated)
            {
                if (Platform.IsUnix() && Platform.IsMonoPlatform())
                    CreateUnix();
                else CreateWin32();
            }

            RedirectConsoleOut();
        }

        private static void CreateWin32()
        {
            if (_allocated)
                return;

            AllocConsole();
            RecreateOutputStream();

            var stdOutHandle = GetStdHandle(StdOutputHandle);

            if (GetConsoleMode(stdOutHandle, out var mode))
            {
                mode |= EnableVirtualTerminalProcessing;
                if (!SetConsoleMode(stdOutHandle, mode))
                {
                    FancyColorsEnabled = false;
                }
            }

            _allocated = true;
        }

        private static void DestroyWin32()
        {
            if (!_allocated)
                return;

            FreeConsole();

            Console.SetOut(_originalStream);
            _allocated = false;
        }

        private static void CreateUnix()
        {
            if (_allocated)
                return;

            RecreateOutputStream();
            _allocated = true;
        }

        private static void DestroyUnix()
        {
            if (!_allocated)
                return;

            RecreateOutputStream();
            _allocated = false;
        }

        private static void RecreateOutputStream()
        {
            _originalStream = Console.Out;
            _outputWriter = new StreamWriter(Console.OpenStandardOutput()) {AutoFlush = true};
            
            RedirectConsoleOut();
        }

        private static void RedirectConsoleOut()
        {
            Console.SetOut(_outputWriter);
        }

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr consoleHandle, out uint mode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr consoleHandle, uint mode);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(int handle);
    }
}