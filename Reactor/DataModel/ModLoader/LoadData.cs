using Reactor.DataModel.Metadata;

namespace Reactor.DataModel.ModLoader
{
    internal class LoadData
    {
        public string RootDirectory { get; set; }
        public ModManifest Manifest { get; set; }
    }
}
