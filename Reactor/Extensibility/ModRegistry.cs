using Reactor.API.DataModel;
using Reactor.API.Logging;
using Reactor.Exceptions;
using System;
using System.Collections.Generic;

namespace Reactor.Extensibility
{
    internal class ModRegistry
    {
        private Log Log => LogManager.GetForCurrentAssembly();

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

        public void InvokeAssetLoaderCallbacks()
        {
            foreach (var mod in Mods)
            {
                if (mod.AssetLoaderMethod != null)
                {
                    Log.Info($"Invoking asset load hook '{mod.AssetLoaderMethod.Name}' for {mod.ModID}...");

                    try
                    {
                        mod.AssetLoaderMethod.Invoke(mod.Instance, new object[] { });
                    }
                    catch (Exception e)
                    {
                        Log.Exception(e);
                    }
                }
            }
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
