using Reactor.API.Configuration;
using UnityEngine;
using GttodTerminal = CommandTerminal.Terminal;

namespace Reactor.API.GTTOD.Internal
{
    internal class Terminal
    {
        private Settings Settings { get; }
        public GttodTerminal GameTerminal { get; private set; }

        internal Terminal(Settings settings)
        {
            Settings = settings;
            CreateGameApiTerminal();

            GttodTerminal.Log("Reactor Game API has been loaded, game terminal overriden.");
        }

        private void CreateGameApiTerminal()
        {
            var termGameObject = new GameObject(Global.GameTerminalNamespace);
            GameObject.DontDestroyOnLoad(termGameObject);

            GameTerminal = termGameObject.AddComponent<GttodTerminal>();
            SetUpTerminalSettings();
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
