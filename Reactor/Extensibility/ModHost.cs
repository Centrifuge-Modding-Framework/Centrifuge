using Reactor.API.DataModel;
using Reactor.DataModel.ModLoader;
using System.Reflection;

namespace Reactor.Extensibility
{
    internal class ModHost
    {
        public Assembly Assembly { get; internal set; }

        public string ModID { get; internal set; }
        public LoadData LoadData { get; internal set; }

        public object GameObject { get; internal set; }
        public object Instance { get; internal set; }

        public ModInfo ToExchangeableApiObject()
        {
            return new ModInfo(
                ModID,
                LoadData.Manifest.FriendlyName,
                LoadData.Manifest.Author,
                LoadData.Manifest.Contact,
                LoadData.Manifest.Priority ?? 10
            );
        }
    }
}
