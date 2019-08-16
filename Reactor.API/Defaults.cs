using System.IO;
using System.Reflection;

namespace Reactor.API
{
    public class Defaults
    {
        private static string CentrifugeRoot => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string PrivateDependencyDirectory => "Dependencies";
        public static string PrivateSettingsDirectory => "Settings";
        public static string PrivateAssetsDirectory => "Assets";
        public static string PrivateDataDirectory => "Data";
        public static string PrivateLogDirectory => "Logs";

        public static string ManagerSettingsDirectory => Path.Combine(CentrifugeRoot, "Settings");
        public static string ManagerModDirectory => Path.Combine(CentrifugeRoot, "Mods");
        public static string ManagerLogDirectory => Path.Combine(CentrifugeRoot, "Logs");

        public const string DependencyResolverLogFileName = "DependencyResolver";
        public const string RuntimeAssetLoaderLogFileName = "RuntimeAssetLoader";
        public const string SettingsSystemLogFileName = "SettingsSystem";
        public const string HotkeyManagerLogFileName = "HotkeyManager";
        public const string FileSystemLogFileName = "FileSystem";
        public const string MessengerLogFileName = "Messenger";
        public const string ModLoaderLogFileName = "Loader";
        public const string ManagerLogFileName = "Manager";

        public const string ManifestFileName = "mod.json";

        public const string ReactorModLoaderNamespace = "com.github.ciastex.ReactorModLoader";
        public const string ReactorGameApiNamespace = "com.github.ciastex.ReactorGameAPI";
    }
}
