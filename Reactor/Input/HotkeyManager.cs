using Reactor.API.Input;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using System;
using System.Collections.Generic;

namespace Reactor.Input
{
    public class HotkeyManager : IHotkeyManager
    {
        private Dictionary<Hotkey, Action> ActionHotkeys { get; }

        private Log Log => LogManager.GetForInternalAssembly();

        public HotkeyManager()
        {
            ActionHotkeys = new Dictionary<Hotkey, Action>();
        }

        public void Bind(Hotkey hotkey, Action action)
        {
            if (Exists(hotkey))
            {
                WriteExistingHotkeyInfo(hotkey);
                return;
            }
            ActionHotkeys.Add(hotkey, action);
            Log.Info($"Bound '{hotkey}' to a mod-defined action.");
        }

        public void Bind(string hotkeyString, Action action)
        {
            Bind(new Hotkey(hotkeyString), action);
        }

        public void Bind(string hotkeyString, Action action, bool isOneTime)
        {
            Bind(new Hotkey(hotkeyString, isOneTime), action);
        }

        public void Unbind(string hotkeyString)
        {
            foreach (var hotkey in ActionHotkeys)
            {
                if (hotkey.ToString() == hotkeyString)
                {
                    ActionHotkeys.Remove(hotkey.Key);
                }
            }
        }

        public void UnbindAll()
        {
            ActionHotkeys.Clear();
        }

        public bool Exists(Hotkey hotkey)
        {
            return ActionHotkeys.ContainsKey(hotkey);
        }

        public bool Exists(string hotkeyString)
        {
            foreach (var hotkey in ActionHotkeys)
            {
                if (hotkey.ToString() == hotkeyString)
                    return true;
            }

            return false;
        }

        public bool IsActionHotkey(Hotkey hotkey)
        {
            return ActionHotkeys.ContainsKey(hotkey);
        }

        internal void Update()
        {
            List<Hotkey> pressed = new List<Hotkey>();
            int biggestPressed = 0;

            if (ActionHotkeys.Count > 0)
            {
                foreach (var hotkey in ActionHotkeys)
                {
                    if (hotkey.Key.IsPressed)
                    {
                        if (hotkey.Key.Specificity > biggestPressed)
                        {
                            pressed.Clear();
                            biggestPressed = hotkey.Key.Specificity;
                        }

                        if (hotkey.Key.Specificity == biggestPressed)
                        {
                            pressed.Add(hotkey.Key);
                        }
                    }
                }
            }

            if (biggestPressed > 0)
            {
                foreach (var hotkey in pressed)
                {
                    ActionHotkeys[hotkey].Invoke();
                }
            }
        }

        private void WriteExistingHotkeyInfo(Hotkey hotkey)
        {
            if (IsActionHotkey(hotkey))
            {
                Log.Error($"The hotkey '{hotkey}' is already bound to another action.");
            }
        }
    }
}
