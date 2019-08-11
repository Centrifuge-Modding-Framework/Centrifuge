using System;
using System.IO;
using System.Reflection;

namespace Centrifuge
{
    public static class Bootstrap
    {
        private static object ReactorManager;
        private static MethodInfo FrameUpdateMethodInfo;

        public static void Initialize()
        {
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                if (arg == StartupArguments.AllocateConsole)
                {
                    if (IsMonoPlatform() && IsUnix())
                    {
                        ConsoleAllocator.CreateUnix();
                        EarlyLog.Info("Running on non-Windows platform. Skipping AllocConsole()...");
                    }
                    else
                    {
                        ConsoleAllocator.CreateWin32();
                    }
                }
            }

            var version = Assembly.GetAssembly(typeof(Bootstrap)).GetName().Version;

            Console.WriteLine($"Centrifuge Mod Loader for GOTTD. Version {version.Major}.{version.Minor}.{version.Build}.{version.Revision}.");
            Console.WriteLine($"Diagnostics mode enabled. Remove '{StartupArguments.AllocateConsole}' command line argument to disable.");
            Console.WriteLine("--------------------------------------------");

            EarlyLog.Info("Trying to find Centrifuge Reactor DLL...");

            var reactorPath = GetCrossPlatformCompatibleReactorPath();
            if (!File.Exists(reactorPath))
            {
                EarlyLog.Error($"Centrifuge Reactor DLL could not be found at '{reactorPath}'. Mods will not be loaded.");
                return;
            }

            EarlyLog.Info("Validating and loading Centrifuge Reactor DLL...");
            var asm = Assembly.LoadFrom(reactorPath);

            var managerType = asm.GetType("Reactor.Manager");
            if (managerType == null)
            {
                EarlyLog.Error($"Invalid Reactor DLL. Could not find the type 'Reactor.Manager'. Mods will not be loaded.");
                return;
            }

            try
            {
                EarlyLog.Info("Activating Reactor Manager instance...");
                ReactorManager = Activator.CreateInstance(managerType);
            }
            catch (Exception e)
            {
                EarlyLog.Exception(e);
                return;
            }

            EarlyLog.Info("Trying to locate the FrameUpdate method...");
            FrameUpdateMethodInfo = ReactorManager.GetType().GetMethod("FrameUpdate");

            if (FrameUpdateMethodInfo == null)
            {
                EarlyLog.Warning("Could not locate the FrameUpdate method, mods will still be loaded, but those relying on frame updates won't work properly.");
            }

            EarlyLog.Info("Passing control to Reactor Manager...");
            var methodInfo = ReactorManager.GetType().GetMethod("Boot");
            if (methodInfo == null)
            {
                EarlyLog.Error("Failed to pass control to Reactor Manager - method 'Boot' was not found. Mods will not be loaded.");
                return;
            }

            Console.WriteLine("--------------------------------------------");
            methodInfo.Invoke(ReactorManager, new object[0]);
        }

        public static void FrameUpdate()
        {
            if (FrameUpdateMethodInfo != null)
            {
                FrameUpdateMethodInfo.Invoke(ReactorManager, new object[0]);
            }
        }

        private static string GetCrossPlatformCompatibleReactorPath()
            => (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "?..?Centrifuge?Reactor.dll".Replace('?', Path.DirectorySeparatorChar));

        private static bool IsMonoPlatform()
        {
            var platformID = (int)Environment.OSVersion.Platform;
            return platformID == 4 || platformID == 6 || platformID == 128;
        }

        private static bool IsUnix()
        {
            var platformID = Environment.OSVersion.Platform;
            switch (platformID)
            {
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    return true;
                default:
                    return false;
            }
        }
    }
}
