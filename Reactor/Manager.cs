﻿using Reactor.API;
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

            Log = new Logger(Defaults.ManagerLogFileName);
            Log.Info("Definitely not up to no good...");

            Hotkeys = new HotkeyManager();
            Messenger = new Messenger();

            ModRegistry = new ModRegistry();
            ModLoader = new ModLoader(this, Defaults.ManagerModDirectory, ModRegistry);

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

        private void InitializeMods()
        {
            ModLoader.Init();
        }
    }
}
