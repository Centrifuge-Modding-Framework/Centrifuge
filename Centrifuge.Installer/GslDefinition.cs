using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Centrifuge.Installer
{
    public class GslDefinition
    {
        [JsonPropertyName("brief")]
        public string Brief { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("data")]
        public Dictionary<string, string> Data { get; set; }
    }
}