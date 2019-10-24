using Reactor.API.DataModel;
using Reactor.Exceptions;
using System.Collections.Generic;

namespace Reactor.Extensibility
{
    internal class ModRegistry
    {
        private List<ModHost> Mods { get; }
        private List<ModInfo> ModInfoCache { get; set; }

        public ModRegistry()
        {
            Mods = new List<ModHost>();
            ModInfoCache = new List<ModInfo>();
        }

        public void RegisterMod(ModHost modHost)
        {
            if (ModIdExists(modHost.ModID))
            {
                throw new DuplicateModIdException(modHost.ModID, "Mod with this ID has already been registered.");
            }

            Mods.Add(modHost);
        }

        public bool ModIdExists(string modId)
            => Mods.Exists(m => m.ModID == modId);

        public List<ModInfo> GetLoadedMods()
        {
            // Cannot remove mods so it's ok to assume mods can only be loaded later
            if (ModInfoCache != null && ModInfoCache.Count == Mods.Count)
                return new List<ModInfo>(ModInfoCache);

            ModInfoCache = new List<ModInfo>();

            foreach (var modHost in Mods)
            {
                ModInfoCache.Add(modHost.ToExchangeableApiObject());
            }

            return new List<ModInfo>(ModInfoCache);
        }
    }
}
