using Reactor.API.DataModel;
using Reactor.API.Events;
using System;
using System.Collections.Generic;

namespace Reactor.API.Interfaces.Systems
{
    public interface IManager
    {
        event EventHandler<ModInitializationEventArgs> ModInitialized;

        IHotkeyManager Hotkeys { get; }

        void SendIPC(string ipcIdentifier, IpcData data);

        bool SetConfig<T>(string key, T value);
        T GetConfig<T>(string key);

        List<ModInfo> GetLoadedMods();
    }
}
