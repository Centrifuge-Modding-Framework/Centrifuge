using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Centrifuge.Installer
{
    public class GslFinder : IDisposable
    {
        private const string GslDefinitionFileLink
            = "https://raw.githubusercontent.com/Centrifuge-Modding-Framework/gsl-artifacts/main/definitions.json";

        private delegate Task GslTaskHandler(GslDefinition def, string gameDataDirectory);

        private Dictionary<string, GslDefinition> _definitions;
        private Dictionary<string, GslTaskHandler> _handlers;

        private HttpClient _client;

        public GslFinder()
        {
            _client = new HttpClient();
            _handlers = new Dictionary<string, GslTaskHandler>
            {
                { "nuget", TryFetchNugetGSL }
            };
        }

        public async Task TryFetchGSL(string assemblyCsharpPath, string gameDataDirectory)
        {
            if (_definitions == null)
                await TryFetchDefinitionFile();

            if (_definitions == null)
                return;

            try
            {
                string hashString;

                using (var sha1 = new SHA1Managed())
                using (var fs = new FileStream(assemblyCsharpPath, FileMode.Open))
                {
                    var hash = sha1.ComputeHash(fs);

                    var sb = new StringBuilder();
                    for (var i = 0; i < hash.Length; i++)
                        sb.Append($"{hash[i]:X2}");

                    hashString = sb.ToString();
                }

                if (!_definitions.ContainsKey(hashString))
                    return;

                var def = _definitions[hashString];

                if (!_handlers.ContainsKey(def.Type))
                {
                    throw new Exception($"No handler for type '{def.Type}' was ever registered.");
                }

                await _handlers[def.Type](def, gameDataDirectory);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Couldn't retrieve and/or unpack GSL: {e.Message}.\n" +
                    $"Centrifuge will still install, but GSLs will have to be installed manually."
                );
            }
        }

        private async Task TryFetchDefinitionFile()
        {
            try
            {
                var json = await _client.GetStringAsync($"{GslDefinitionFileLink}?nocache={DateTime.Now}");
                _definitions = JsonSerializer.Deserialize<Dictionary<string, GslDefinition>>(json);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Couldn't retrieve and/or parse GSL definitions: {e.Message}.\n" +
                    $"Centrifuge will still install, but GSLs will have to be installed manually."
                );
            }
        }

        private async Task TryFetchNugetGSL(GslDefinition def, string gameDataDirectory)
        {
            var nugetLink = def.Data["nuget_link"];
            var version = def.Data["version"];
            var zipPath = def.Data["zip_path"];
            var outPath = def.Data["out_path"];

            using (var stream = await _client.GetStreamAsync($"{nugetLink}/{version}"))
            {
                using (var zip = new ZipArchive(stream))
                {
                    var entry = zip.GetEntry(zipPath);
                    var actualOutPath = Path.Combine(
                        gameDataDirectory,
                        outPath
                    );
                    Directory.CreateDirectory(Path.GetDirectoryName(actualOutPath));
                    entry.ExtractToFile(actualOutPath);
                }
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}