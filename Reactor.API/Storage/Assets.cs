using Centrifuge.UnityInterop.Bridges;
using Reactor.API.Logging;
using System;
using System.IO;
using System.Reflection;

namespace Reactor.API.Storage
{
    public class Assets
    {
        private string _filePath = null;

        private string RootDirectory { get; }
        private string FileName { get; set; }
        private string FilePath => _filePath ?? Path.Combine(Path.Combine(RootDirectory, Defaults.PrivateAssetsDirectory), FileName);

        private static Log Log => LogManager.GetForInternalAssembly();

        public object Bundle { get; private set; }

        private Assets() { }

        /// <summary>
        /// Attempts to construct a Unity AssetBundle via a Centrifuge Type Bridge. 
        /// You will have to cast the Bundle property to Unity's AssetBundle type for usage.
        /// </summary>
        /// <param name="fileName">Filename/path relative to mod's private asset directory.</param>
        public Assets(string fileName)
        {
            RootDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            FileName = fileName;

            if (!File.Exists(FilePath))
            {
                Log.Error($"Couldn't find requested asset bundle at {FilePath}");
                return;
            }

            Bundle = Load();
        }

        /// <summary>
        /// Attempts to construct a Unity AssetBundle via a Centrifuge Type Bridge.
        /// You will have to cast the Bundle property to Unity's AssetBundle type for usage.
        /// </summary>
        /// <param name="filePath">An absolute path to the AssetBundle</param>
        public static Assets FromUnsafePath(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Log.Error($"Could not find requested asset bundle at {filePath}");
                return null;
            }

            var ret = new Assets
            {
                _filePath = filePath,
                FileName = Path.GetFileName(filePath)
            };
            ret.Bundle = ret.Load();

            if (ret.Bundle == null)
                return null;

            return ret;
        }

        private object Load()
        {
            try
            {
                var assetBundle = AssetBundleBridge.LoadFrom(FilePath);
                Log.Info($"Loaded asset bundle {FilePath}");

                return assetBundle;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return null;
            }
        }
    }
}
