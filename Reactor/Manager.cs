using Reactor.API;
using Reactor.API.Configuration;
using Reactor.API.DataModel;
using Reactor.API.Events;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using Reactor.Communication;
using Reactor.Extensibility;
using Reactor.Input;
using System;
using System.Collections.Generic;

namespace Reactor
{
    public class Manager : IManager
    {
        private static Logger Log { get; set; }
        private static bool InterceptUnityLogs { get; set; }

        private GameSupport GameSupport { get; set; }
        private ModLoader ModLoader { get; set; }
        private ModRegistry ModRegistry { get; set; }

        public IHotkeyManager Hotkeys { get; private set; }
        public IMessenger Messenger { get; private set; }

        public event EventHandler<ModInitializationEventArgs> ModInitialized;
        public event EventHandler InitFinished;

        public Manager()
        {
            InitializeSettings();
            InitializeLogger();

            Log.Info("Spooling up!");

            Hotkeys = new HotkeyManager();
            Messenger = new Messenger();

            GameSupport = new GameSupport();
            ModRegistry = new ModRegistry();
            ModLoader = new ModLoader(this, Defaults.ManagerModDirectory, ModRegistry);

            InitializeGameSupport();
            InitializeMods();
        }

        public void Update()
        {
            ((HotkeyManager)Hotkeys).Update();
        }

        public List<ModInfo> GetLoadedMods()
        {
            return ModRegistry.GetLoadedMods();
        }

        internal void OnModInitialized(ModInfo modInfo)
        {
            ModInitialized?.Invoke(this, new ModInitializationEventArgs(modInfo));
        }

        internal void OnInitFinished()
        {
            InitFinished?.Invoke(this, EventArgs.Empty);
        }

        private void InitializeSettings()
        {
            Global.Settings = new Settings("reactor");
            Global.Settings.GetOrCreate(Global.InterceptUnityLogsSettingsKey, true);

            if (Global.Settings.Dirty)
            {
                Global.Settings.Save();
            }
        }

        private void InitializeLogger()
        {
            Log = new Logger(Defaults.ManagerLogFileName);
            InterceptUnityLogs = Global.Settings.GetItem<bool>(Global.InterceptUnityLogsSettingsKey);
        }

        private void InitializeGameSupport()
        {
            Log.Info("Initializing game support.");
            GameSupport.Initialize();
        }

        private void InitializeMods()
        {
            Log.Info("Initializing mods.");
            ModLoader.Init();
        }

        public void LogUnityEngineMessage(string condition, string stackTrace, int logType)
        {
            if (!InterceptUnityLogs)
                return;

            var msg = $"[::UNITY::] {condition}";

            if (!string.IsNullOrEmpty(stackTrace))
            {
                msg += $"\n{stackTrace}";
            }

            switch (logType)
            {
                case 0: // LogType.Error
                case 1: // LogType.Assert
                case 4: // LogType.Exception
                    Log.Error(msg);
                    break;

                case 2:
                    Log.Warning(msg);
                    break;

                case 3:
                    Log.Info(msg);
                    break;
            }
        }
    }
}
