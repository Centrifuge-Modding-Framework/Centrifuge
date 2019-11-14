using Reactor.API;
using Reactor.API.Configuration;
using Reactor.API.DataModel;
using Reactor.API.Events;
using Reactor.API.Interfaces.Systems;
using Reactor.Communication;
using Reactor.Extensibility;
using Reactor.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reactor
{
    public class Manager : IManager
    {
        private GameSupport GameSupport { get; set; }
        private ModRegistry ModRegistry { get; set; }
        private ModLoader ModLoader { get; set; }

        public UnityLog UnityLog { get; }

        public IHotkeyManager Hotkeys { get; private set; }
        public IMessenger Messenger { get; private set; }

        public event EventHandler<ModInitializationEventArgs> ModInitialized;
        public event EventHandler InitFinished;

        public Manager()
        {
            InitializeSettings();

            UnityLog = new UnityLog();

            Hotkeys = new HotkeyManager();
            Messenger = new Messenger();

            GameSupport = new GameSupport(this);
            ModRegistry = new ModRegistry();
            ModLoader = new ModLoader(this, Defaults.ManagerModDirectory, ModRegistry);

            GameSupport.Initialize();
            ModLoader.Initialize();
        }

        public List<ModInfo> GetLoadedMods()
        {
            return ModRegistry.GetLoadedMods();
        }

        public List<string> GetLoadedGslIds()
        {
            return Global.GameApiObjects.Keys.ToList();
        }

        private void InitializeSettings()
        {
            Global.Settings = new Settings("reactor");

            Global.InterceptUnityLogs = Global.Settings.GetOrCreate(Global.InterceptUnityLogsSettingsKey, true);

            Global.Settings.SaveIfDirty();
        }

        internal void OnModInitialized(ModInfo modInfo)
        {
            ModInitialized?.Invoke(this, new ModInitializationEventArgs(modInfo));
        }

        internal void OnInitFinished()
        {
            InitFinished?.Invoke(this, EventArgs.Empty);
        }

        public void Update()
        {
            ((HotkeyManager)Hotkeys).Update();
        }
    }
}
