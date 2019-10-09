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
using UnityEngine.SceneManagement;

namespace Reactor
{
    public class Manager : UnityEngine.MonoBehaviour, IManager
    {
        private Logger Log { get; set; }

        private GameSupport GameSupport { get; set; }
        private ModRegistry ModRegistry { get; set; }
        private ModLoader ModLoader { get; set; }

        public IHotkeyManager Hotkeys { get; private set; }
        public IMessenger Messenger { get; private set; }

        public event EventHandler<ModInitializationEventArgs> ModInitialized;
        public event EventHandler InitFinished;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Log.Info("Spooling up!");

            InitializeSettings();
            InitializeLogger();

            Hotkeys = new HotkeyManager();
            Messenger = new Messenger();

            GameSupport = new GameSupport();
            ModRegistry = new ModRegistry();
            ModLoader = new ModLoader(this, Defaults.ManagerModDirectory, ModRegistry);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
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

        private void InitializeGameSupport()
        {
            GameSupport.Initialize();
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

            if (Global.Settings.GetItem<bool>(Global.InterceptUnityLogsSettingsKey))
            {
                UnityEngine.Application.logMessageReceived += Application_logMessageReceived;
            }
        }

        private void InitializeMods()
        {
            ModLoader.Init();
        }

        private void Application_logMessageReceived(string condition, string stackTrace, UnityEngine.LogType type)
        {
            var msg = $"[::UNITY::] {condition}";

            if (!string.IsNullOrEmpty(stackTrace))
            {
                msg += $"\n{stackTrace}";
            }

            switch (type)
            {
                case UnityEngine.LogType.Exception:
                case UnityEngine.LogType.Assert:
                case UnityEngine.LogType.Error:
                    Log.Error(msg);
                    break;

                case UnityEngine.LogType.Log:
                    Log.Info(msg);
                    break;

                case UnityEngine.LogType.Warning:
                    Log.Warning(msg);
                    break;
            }
        }
    }
}
