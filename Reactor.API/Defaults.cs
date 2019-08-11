using System.IO;
using System.Reflection;

namespace Reactor.API
{
    public class Defaults
    {
        private static string BasePath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string PrivateDependencyDirectory => "Dependencies";
        public static string PrivateSettingsDirectory => "Settings";
        public static string PrivateAssetsDirectory => "Assets";
        public static string PrivateDataDirectory => "Data";
        public static string PrivateLogDirectory => "Logs";

        public static string ManagerSettingsDirectory => Path.Combine(BasePath, "Settings");
        public static string ManagerPluginDirectory => Path.Combine(BasePath, "Mods");
        public static string ManagerLogDirectory => Path.Combine(BasePath, "Logs");

        public const string DependencyResolverLogFileName = "DependencyResolver";
        public const string RuntimeAssetLoaderLogFileName = "RuntimeAssetLoader";
        public const string SettingsSystemLogFileName = "SettingsSystem";
        public const string HotkeyManagerLogFileName = "HotkeyManager";
        public const string FileSystemLogFileName = "FileSystem";
        public const string ModLoaderLogFileName = "Loader";
        public const string ManagerLogFileName = "Manager";
    }
}
