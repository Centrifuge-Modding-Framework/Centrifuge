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

        public static string ManagerSettingsDirectory => Path.Combine(CentrifugeRoot, "Settings");
        public static string ManagerGameSupportDirectory => Path.Combine(CentrifugeRoot, "GameSupport");
        public static string ManagerModDirectory => Path.Combine(CentrifugeRoot, "Mods");
        public static string ManagerLogDirectory => Path.Combine(CentrifugeRoot, "Logs");

        public const string ManifestFileName = "mod.json";

        public const string ReactorModLoaderNamespace = "com.github.ciastex.ReactorModLoader";
    }
}
