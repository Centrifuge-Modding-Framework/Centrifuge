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
        private static Log Log { get; set; }

        private GameSupport GameSupport { get; set; }
        private ModLoader ModLoader { get; set; }
        private ModRegistry ModRegistry { get; set; }

        public IHotkeyManager Hotkeys { get; private set; }
        public IMessenger Messenger { get; private set; }

        public string LoadedGameSupportID => GameSupport.GameSupportID;

        public event EventHandler<ModInitializationEventArgs> ModInitialized;
        public event EventHandler InitFinished;

        public Manager()
        {
            InitializeSettings();
            InitializeLogger();

            Hotkeys = new HotkeyManager();
            Messenger = new Messenger();

            GameSupport = new GameSupport();
            ModRegistry = new ModRegistry();
            ModLoader = new ModLoader(this, Defaults.ManagerModDirectory, ModRegistry);

            GameSupport.Initialize();
            ModLoader.Initialize();
        }

        public List<ModInfo> GetLoadedMods()
        {
            return ModRegistry.GetLoadedMods();
        }

        private void InitializeSettings()
        {
            Global.Settings = new Settings("reactor");
            Global.InterceptUnityLogs = Global.Settings.GetOrCreate(Global.InterceptUnityLogsSettingsKey, true);

            if (Global.Settings.Dirty)
            {
                Global.Settings.Save();
            }
        }

        private void InitializeLogger()
        {
            Log = new Log(Defaults.ManagerLogFileName);
            Log.Info("Spooling up!");
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

        public void LogUnityEngineMessage(string condition, string stackTrace, int logType)
        {
            if (!Global.InterceptUnityLogs)
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
