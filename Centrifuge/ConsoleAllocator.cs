using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Centrifuge
{
    internal static class ConsoleAllocator
    {
        private static StreamWriter _outputWriter;
        private static TextWriter _originalStream;

        private static bool _allocated;

        public static void CreateWin32()
        {
            if (_allocated)
                return;

            AllocConsole();
            RecreateOutputStream();
            _allocated = true;
        }
        public static void DestroyWin32()
        {
            if (!_allocated)
                return;

            FreeConsole();

            Console.SetOut(_originalStream);
            _allocated = false;
        }

        public static void CreateUnix()
        {
            if (_allocated)
                return;

            RecreateOutputStream();
            _allocated = true;
        }

        public static void DestroyUnix()
        {
            if (!_allocated)
                return;

            RecreateOutputStream();
            _allocated = false;
        }

        private static void RecreateOutputStream()
        {
            _originalStream = Console.Out;
            _outputWriter = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
            Console.SetOut(_outputWriter);
        }

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();
    }
}
