using LitJson;
using Reactor.API.Exceptions;
using System;
using System.IO;
using System.Text;

namespace Reactor.DataModel.Metadata
{
    internal class ModManifest
    {
        public string FriendlyName { get; protected set; }
        public string Author { get; protected set; }
        public string Contact { get; protected set; }
        public string ModuleFileName { get; protected set; }
        public string[] Dependencies { get; protected set; }
        public int? Priority { get; protected set; }
        public bool SkipLoad { get; protected set; }

        public ModManifest() { }

        public static ModManifest FromFile(string filePath)
        {
            string json;

            try
            {
                using (var sr = new StreamReader(filePath))
                {
                    json = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw new ManifestReadException("Failed to open the manifest file.", true, string.Empty, ex);
            }

            try
            {
                var manifest = JsonMapper.ToObject<ModManifest>(json);

                if (manifest == null)
                {
                    throw new ManifestReadException("JSON deserializer returned null.", false, json);
                }

                if (manifest.Priority == null)
                {
                    manifest.Priority = 10;
                }

                return manifest;
            }
            catch (JsonException je)
            {
                throw new ManifestReadException("Failed to deserialize JSON data.", false, json, je);
            }
            catch (Exception e)
            {
                throw new ManifestReadException("Unexpected metadata read exception occured.", false, json, e);
            }
        }

        public ManifestValidationFlags Validate()
        {
            ManifestValidationFlags flags = 0;

            if (string.IsNullOrEmpty(FriendlyName))
            {
                flags |= ManifestValidationFlags.MissingFriendlyName;
            }

            if (string.IsNullOrEmpty(ModuleFileName))
            {
                flags |= ManifestValidationFlags.MissingModuleFileName;
            }

            return flags;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Name: {FriendlyName}");
            sb.AppendLine($"Module file name: {ModuleFileName}");

            if (!string.IsNullOrEmpty(Author))
            {
                sb.AppendLine($"By: {Author}");
            }

            if (!string.IsNullOrEmpty(Contact))
            {
                sb.AppendLine($"Contact: {Contact}");
            }

            if (Dependencies != null && Dependencies.Length > 0)
            {
                sb.AppendLine($"Declared dependencies: ");
                foreach (var str in Dependencies)
                {
                    sb.AppendLine($"  {str}");
                }
            }

            return sb.ToString();
        }
    }
}
