using Reactor.API.Configuration;
using UnityEngine;
using GttodTerminal = CommandTerminal.Terminal;

namespace Reactor.API.GTTOD.Internal
{
    internal class Terminal
    {
        internal static bool IsInitializing { get; private set; } = true;

        private Settings Settings { get; }
        public GttodTerminal GameTerminal { get; private set; }

        internal Terminal(Settings settings)
        {
            Settings = settings;
            CreateGameApiTerminal();
        }

        private void CreateGameApiTerminal()
        {
            var termGameObject = new GameObject(Global.GameTerminalNamespace);
            GameObject.DontDestroyOnLoad(termGameObject);

            GameTerminal = termGameObject.AddComponent<GttodTerminal>();
            SetUpTerminalSettings();

            IsInitializing = false;
        }

        private void SetUpTerminalSettings()
        {
            GameTerminal.ConsoleFont = Font.CreateDynamicFontFromOSFont(
                Settings.GetItem<string>(Global.ConsoleFontNameSettingsKey),
                Settings.GetItem<int>(Global.ConsoleFontSizeSettingsKey)
            );

            GameTerminal.ToggleSpeed = Settings.GetItem<int>(Global.ConsoleDropDownAnimationSpeedSettingsKey);
            GameTerminal.BufferSize = Settings.GetItem<int>(Global.ConsoleBufferSizeSettingsKey);
            GameTerminal.ShowGUIButtons = Settings.GetItem<bool>(Global.ConsoleShowGuiButtonsSettingsKey);
        }
    }
}
