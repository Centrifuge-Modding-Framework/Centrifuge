using Centrifuge.UnityInterop.Bridges;
using Centrifuge.UnityInterop.Builders;
using Centrifuge.UnityInterop.DataModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Centrifuge
{
    public static class Bootstrap
    {
        private static object ReactorManagerObject;
        private static bool ConsoleEnabled { get; set; }

        static Bootstrap()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

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

                    ConsoleEnabled = true;
                }
            }

            var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

            EarlyLog.Info($"Centrifuge bootstrap for Reactor Mod Loader and API. Version {version}. Unity {ApplicationBridge.UnityVersion}");

            if (ConsoleEnabled)
                EarlyLog.Info($"Diagnostics mode enabled. Remove '{StartupArguments.AllocateConsole}' command line argument to disable.");

            if (ApplicationBridge.GetRunningUnityGeneration() == UnityGeneration.Unity4OrOlder)
            {
                EarlyLog.Error("Centrifuge requires Unity 5 or newer. Terminating.");
                return;
            }

            EarlyLog.Info("Trying to find Centrifuge Reactor DLL...");
            var reactorPath = GetCrossPlatformCompatibleReactorPath();
            if (!File.Exists(reactorPath))
            {
                EarlyLog.Error($"Centrifuge Reactor DLL could not be found at '{reactorPath}'. Mods will not be loaded.");
                return;
            }

            Type proxyType;
            try
            {
                EarlyLog.Info("Validating and loading Centrifuge Reactor DLL...");
                Assembly.LoadFrom(reactorPath);

                EarlyLog.Info("Building manager proxy component for Unity Engine...");
                proxyType = new ManagerProxyBuilder().Build();
            }
            catch (ReflectionTypeLoadException rtle)
            {
                EarlyLog.Exception(rtle);

                EarlyLog.Info(" --- LOADER EXCEPTIONS FOLLOW --- ");
                foreach (var lex in rtle.LoaderExceptions)
                {
                    EarlyLog.Exception(lex);
                }
                return;
            }
            catch (Exception ex)
            {
                EarlyLog.Exception(ex);
                return;
            }

            try
            {
                EarlyLog.Info("Creating Reactor Manager GameObject...");
                ReactorManagerObject = GameObjectBridge.CreateGameObject("com.github.ciastex/ReactorModLoaderProxy");
            }
            catch (Exception e)
            {
                EarlyLog.Exception(e);
                return;
            }

            EarlyLog.Info("About to add component to Reactor Manager GameObject.");
            object proxyComponent;
            try
            {
                proxyComponent = GameObjectBridge.AttachComponentTo(ReactorManagerObject, proxyType);
            }
            catch (Exception e)
            {
                EarlyLog.Exception(e);
                return;
            }

            if (proxyComponent == null)
            {
                EarlyLog.Error("Manager proxy component has failed to attach when it wasn't really supposed to fail on Unity 5+.");

                EarlyLog.Info("Report this stuff to https://github.com/Ciastex/Centrifuge/issues.");
                EarlyLog.Info("Make sure to check for and - if existing - include your game's output_log.txt (and/or Player.log) in the report.");
                EarlyLog.Info("Definitely include this entire log as well.");
                EarlyLog.Info("Otherwise I'll be very angry and ask you for this stuff in a very rude manner.");

                if (IsUnix())
                {
                    EarlyLog.Info("Look in ~/.config/unity3d/<CompanyName>/<GameName>/ for any .log and/or .txt files.");
                }
                else
                {
                    var path = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "\\AppData\\LocalLow");
                    EarlyLog.Info($"Look in {path}\\<CompanyName>\\<GameName> for any .log and/or .txt files.");
                }
            }

            EarlyLog.Info(" --- BOOTSTRAPPER FINISHED --- ");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            EarlyLog.Error("FATAL: Unhandled exception!");
            EarlyLog.Exception(e.ExceptionObject as Exception);
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
