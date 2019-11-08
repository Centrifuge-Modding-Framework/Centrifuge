using System.IO;
using System.Reflection;

namespace Reactor.API
{
    public static class Defaults
    {
        public static string CentrifugeRoot => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string PrivateDependencyDirectory => "Dependencies";
        public static string PrivateSettingsDirectory => "Settings";
        public static string PrivateAssetsDirectory => "Assets";
        public static string PrivateDataDirectory => "Data";
        public static string PrivateLogDirectory => "Logs";

        public static string ManagerSettingsDirectory => Path.Combine(CentrifugeRoot, "Settings");
        public static string ManagerModDirectory => Path.Combine(CentrifugeRoot, "Mods");
        public static string ManagerLogDirectory => Path.Combine(CentrifugeRoot, "Logs");

        public static string GameSupportLibraryPath => Path.Combine(CentrifugeRoot, GameSupportLibraryFilePattern);
        public static string ConsolidatedLogFilePath => Path.Combine(ManagerLogDirectory, ConsolidatedLogFileName);

        public const string DependencyResolverLogFileName = "DependencyResolver";
        public const string RuntimeAssetLoaderLogFileName = "RuntimeAssetLoader";
        public const string SettingsSystemLogFileName = "SettingsSystem";
        public const string HotkeyManagerLogFileName = "HotkeyManager";
        public const string FileSystemLogFileName = "FileSystem";
        public const string MessengerLogFileName = "Messenger";
        public const string ModLoaderLogFileName = "Loader";
        public const string ManagerLogFileName = "Manager";
        public const string GameSupportInitializerLogFileName = "GameSupport";

        public const string GameSupportLibraryFilePattern = "*.GameSupport.dll";
        public const string ManifestFileName = "mod.json";
        public const string ConsolidatedLogFileName = "CommonLog.txt";

        public const string ReactorModLoaderNamespace = "com.github.ciastex.ReactorModLoader";
    }
}
