using Reactor.API.DataModel;
using Reactor.API.Events;
using System;
using System.Collections.Generic;

namespace Reactor.API.Interfaces.Systems
{
    public interface IManager
    {
        event EventHandler<ModInitializationEventArgs> ModInitialized;
        event EventHandler InitFinished;

        IHotkeyManager Hotkeys { get; }
        IMessenger Messenger { get; }

        string LoadedGameSupportID { get; }

        List<ModInfo> GetLoadedMods();
    }
}
