using Reactor.API;
using Reactor.API.DataModel;
using Reactor.API.Events;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using System;
using System.Collections.Generic;

namespace Reactor
{
    public class Manager : IManager
    {
        private Logger Log { get; set; }

        public IHotkeyManager Hotkeys => throw new NotImplementedException();

        public event EventHandler<ModInitializationEventArgs> ModInitialized;

        public Manager()
        {

        }

        public void Boot()
        {
            Log = new Logger(Defaults.ManagerLogFileName);
            Log.Info("Definitely not up to no good...");
        }

        public void FrameUpdate()
        {
            Log.Info("Nyooom.");
        }

        public List<ModInfo> GetLoadedMods()
        {
            throw new NotImplementedException();
        }

        public void SendIPC(string ipcIdentifier, IpcData data)
        {
            throw new NotImplementedException();
        }

        public T GetConfig<T>(string key)
        {
            throw new NotImplementedException();
        }

        public bool SetConfig<T>(string key, T value)
        {
            throw new NotImplementedException();
        }
    }
}
