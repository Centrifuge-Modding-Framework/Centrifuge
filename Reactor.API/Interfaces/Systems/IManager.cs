using System;
using System.Collections.Generic;
using Reactor.API.DataModel;
using Reactor.API.Events;

namespace Reactor.API.Interfaces.Systems
{
    public interface IManager
    {
        event EventHandler<ModInitializationEventArgs> ModInitialized;
        event EventHandler InitFinished;
        event EventHandler GslInitFinished;

        IHotkeyManager Hotkeys { get; }
        
        [Obsolete("Use IManager.GetMod(string modId).Instance to communicate between the mods instead." +
                  "\nThis will be removed in Centrifuge 4.0.")]
        IMessenger Messenger { get; }

        ModInfo GetMod(string modId);
        List<ModInfo> GetLoadedMods();
        List<string> GetLoadedGslIds();
    }
}
