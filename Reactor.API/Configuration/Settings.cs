using LitJson;
using Reactor.API.Logging;
using System;
using System.IO;
using System.Reflection;

namespace Reactor.API.Configuration
{
    public class Settings : Section
    {
        private static Log Log => LogManager.GetForInternalAssembly();

        private string FileName { get; }
        private string RootDirectory { get; }
        private string SettingsDirectory => Path.Combine(RootDirectory, Defaults.PrivateSettingsDirectory);
        private string FilePath => Path.Combine(SettingsDirectory, FileName);

        public Settings(string fileName)
        {
            RootDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

            FileName = $"{fileName}.json";

            Log.Debug($"Settings instance for '{FilePath}' initializing...");

            if (File.Exists(FilePath))
            {
                using (var sr = new StreamReader(FilePath))
                {
                    var json = sr.ReadToEnd();
                    Section sec = null;

                    try
                    {
                        var data = JsonMapper.ToObject<Section>(json);
                    }
                    catch (JsonException je)
                    {
                        Log.Exception(je);
                    }
                    catch (Exception e)
                    {
                        Log.Exception(e);
                    }

                    if (sec != null)
                    {
                        foreach (string k in sec.Keys)
                            Add(k, sec[k]);
                    }
                }
            }
            Dirty = false;
        }

        public void SaveIfDirty(bool formatJson = true)
        {
            if (Dirty)
                Save(formatJson);
        }

        public void Save(bool formatJson = true)
        {
            if (!Directory.Exists(SettingsDirectory))
                Directory.CreateDirectory(SettingsDirectory);

            try
            {
                using (var sw = new StreamWriter(FilePath, false))
                {
                    var jw = new JsonWriter(sw)
                    {
                        PrettyPrint = formatJson,
                        IndentValue = 2
                    };

                    JsonMapper.ToJson(this, jw);
                }

                Dirty = false;
            }
            catch (JsonException je)
            {
                Log.Exception(je);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }
    }
}
