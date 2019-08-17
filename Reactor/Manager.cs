﻿using Reactor.API;
using Reactor.API.Configuration;
using Reactor.API.DataModel;
using Reactor.API.Events;
using Reactor.API.Extensions;
using Reactor.API.GTTOD;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using Reactor.Communication;
using Reactor.Extensibility;
using Reactor.Input;
using System;
using System.Collections.Generic;

namespace Reactor
{
    public class Manager : UnityEngine.MonoBehaviour, IManager
    {
        private Logger Log { get; set; }

        private ModRegistry ModRegistry { get; set; }
        private ModLoader ModLoader { get; set; }

        public IHotkeyManager Hotkeys { get; private set; }
        public IMessenger Messenger { get; private set; }

        public event EventHandler<ModInitializationEventArgs> ModInitialized;
        public event EventHandler InitFinished;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);

            InitializeSettings();
            InitializeLogger();

            Log.Info("Definitely not up to no good...");

            Hotkeys = new HotkeyManager();
            Messenger = new Messenger();

            ModRegistry = new ModRegistry();
            ModLoader = new ModLoader(this, Defaults.ManagerModDirectory, ModRegistry);

            Global.GameApiObject = new UnityEngine.GameObject(Defaults.ReactorGameApiNamespace);
            Global.GameApiObject.AddComponent<GameAPI>();

            AddCetrifugeSpecificCommands();

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

            if (Global.Settings.GetItem<bool>(Global.InterceptUnityLogsSettingsKey))
            {
                UnityEngine.Application.logMessageReceived += Application_logMessageReceived;
            }
        }

        private void InitializeMods()
        {
            ModLoader.Init();
        }

        private void AddCetrifugeSpecificCommands()
        {
            CommandTerminal.Terminal.Shell.AddCommand("cnfg_version", (args) =>
            {
                var reactorAssembly = AssemblyEx.GetAssemblyByName("Reactor");
                var centrifugeAssembly = AssemblyEx.GetAssemblyByName("Centrifuge");
                var reactorApiAssembly = AssemblyEx.GetAssemblyByName("Reactor.API");
                var reactorGameApiAssembly = AssemblyEx.GetAssemblyByName("Reactor.API.GTTOD");

                CommandTerminal.Terminal.Log($"Reactor ModLoader version: {reactorAssembly.GetName().Version.ToString()}");
                CommandTerminal.Terminal.Log($"Reactor GameAPI version: {reactorGameApiAssembly.GetName().Version.ToString()}");
                CommandTerminal.Terminal.Log($"Reactor API version: {reactorApiAssembly.GetName().Version.ToString()}");
                CommandTerminal.Terminal.Log($"Centrifuge version: {centrifugeAssembly.GetName().Version.ToString()}");
            }, 0, -1, "Prints versions of all Centrifuge modules.");
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
