using Reactor.API.Configuration;
using Reactor.API.GTTOD.Events.Args;
using System;
using UnityEngine;
using GttodTerminal = CommandTerminal.Terminal;

namespace Reactor.API.GTTOD.Internal
{
    public class Terminal
    {
        private Settings Settings { get; }
        internal GttodTerminal GameTerminal { get; private set; }

        public static event EventHandler InitFinished;

        public static event EventHandler<TerminalStyleEventArgs> LabelStyleSet;
        public static event EventHandler<TerminalStyleEventArgs> WindowStyleSet;
        public static event EventHandler<TerminalStyleEventArgs> InputStyleSet;

        public static event EventHandler<TerminalStateEventArgs> Opened;
        public static event EventHandler<TerminalStateEventArgs> Closed;

        internal Terminal(Settings settings)
        {
            Settings = settings;
            LabelStyleSet += Terminal_LabelStyleSet;

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

        private void Terminal_LabelStyleSet(object sender, TerminalStyleEventArgs e)
        {
            e.Style.padding = new RectOffset(0, 0, 0, 4);
        }

        internal static void OnInitFinished()
            => InitFinished?.Invoke(null, EventArgs.Empty);

        internal static void OnWindowStyleSet(GUIStyle style)
            => WindowStyleSet?.Invoke(null, new TerminalStyleEventArgs(style));

        internal static void OnInputStyleSet(GUIStyle style)
            => InputStyleSet?.Invoke(null, new TerminalStyleEventArgs(style));

        internal static void OnLabelStyleSet(GUIStyle style)
            => LabelStyleSet?.Invoke(null, new TerminalStyleEventArgs(style));

        internal static void OnClosed(GttodTerminal terminal)
            => Closed?.Invoke(null, new TerminalStateEventArgs(terminal));

        internal static void OnOpened(GttodTerminal terminal)
            => Opened?.Invoke(null, new TerminalStateEventArgs(terminal));
    }
}
