using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Centrifuge.UnityInterop.Bridges;
using Reactor.API;
using Reactor.API.Configuration;
using Reactor.API.DataModel;
using Reactor.API.Events;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using Reactor.Communication;
using Reactor.Extensibility;
using Reactor.Input;

namespace Reactor
{
    public class Manager : IManager
    {
        private Log Log { get; } = LogManager.GetForCurrentAssembly();
        
        private GameSupportLoader GameSupportLoader { get; set; }
        private ModRegistry ModRegistry { get; set; }
        private ModLoader ModLoader { get; set; }

        internal static Settings Settings { get; private set; }

        public UnityLog UnityLog { get; }
        public HarmonyXLog HarmonyXLog { get; }

        public IHotkeyManager Hotkeys { get; private set; }

        [Obsolete("Use IManager.GetMod(string modId).Instance to communicate between the mods instead." +
                  "\nThis will be removed in Centrifuge 4.0.")]
        public IMessenger Messenger { get; private set; }

        public event EventHandler<ModInitializationEventArgs> ModInitialized;
        public event EventHandler InitFinished;
        public event EventHandler GslInitFinished;

        public Manager()
        {
            InitializeSettings();

            UnityLog = new UnityLog();
            HarmonyXLog = new HarmonyXLog();

            Hotkeys = new HotkeyManager();
            Messenger = new Messenger();

            GameSupportLoader = new GameSupportLoader(this);
            
            ModRegistry = new ModRegistry();
            ModLoader = new ModLoader(this, GetModRepositoryPath(), ModRegistry);

            GameSupportLoader.Initialize();
            ModLoader.Initialize();
        }

        public ModInfo GetMod(string modId)
            => GetLoadedMods().FirstOrDefault(m => m.ModID == modId);

        public List<ModInfo> GetLoadedMods()
            => ModRegistry.GetLoadedMods();

        public List<string> GetLoadedGslIds()
            => GameSupportLoader.GSLs.Select(x => x.ID).ToList();

        public void Update()
            => ((HotkeyManager)Hotkeys).Update();

        public void CallAssetLoadHooks()
        {
            SceneManagerBridge.DetachSceneLoadedEventHandler();
            ModRegistry.InvokeAssetLoaderCallbacks();
        }

        internal void OnModInitialized(ModInfo modInfo)
            => ModInitialized?.Invoke(this, new ModInitializationEventArgs(modInfo));

        internal void OnInitFinished()
            => InitFinished?.Invoke(this, EventArgs.Empty);

        internal void OnGslInitFinished()
            => GslInitFinished?.Invoke(this, EventArgs.Empty);

        private void InitializeSettings()
        {
            Settings = new Settings("reactor");
            Settings.GetOrCreate(Resources.InterceptUnityLogsSettingsKey, true);
            Settings.GetOrCreate(Resources.InterceptHarmonyXLogsSettingsKey, true);
            Settings.GetOrCreate(Resources.OverrideModRepositoryPathSettingsKey, false);
            Settings.GetOrCreate(Resources.ExternalModRepositoryPathSettingsKey, string.Empty);

            Settings.SaveIfDirty();
        }

        private string GetModRepositoryPath()
        {
            var path = Settings.GetItem<bool>(Resources.OverrideModRepositoryPathSettingsKey)
                ? Settings.GetItem<string>(Resources.ExternalModRepositoryPathSettingsKey)
                : Defaults.ManagerModDirectory;

            if (!Directory.Exists(path))
            {
                Log.Warning($"The path '{path}' does not exist... Defaults loaded.");
                path = Defaults.ManagerModDirectory;
            }

            return path;
        }
    }
}