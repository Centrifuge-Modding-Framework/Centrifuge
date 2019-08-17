using Harmony;
using Reactor.API.Configuration;
using Reactor.API.GTTOD.Internal;
using System.Reflection;
using UnityEngine;

namespace Reactor.API.GTTOD
{
    public class GameAPI : MonoBehaviour
    {
        private Settings Settings { get; set; }

        internal HarmonyInstance HarmonyInstance { get; private set; }
        internal Terminal Terminal { get; private set; }

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);

            InitializeSettings();
            InitializeMixins();

            Terminal = new Terminal(Settings);
        }

        private void InitializeSettings()
        {
            Settings = new Settings("game_api");

            Settings.GetOrCreate(Global.ConsoleFontNameSettingsKey, "Lucida Console");
            Settings.GetOrCreate(Global.ConsoleFontSizeSettingsKey, 13);
            Settings.GetOrCreate(Global.ConsoleBufferSizeSettingsKey, 1024);
            Settings.GetOrCreate(Global.ConsoleDropDownAnimationSpeedSettingsKey, 720);
            Settings.GetOrCreate(Global.ConsoleShowGuiButtonsSettingsKey, false);

            if (Settings.Dirty)
                Settings.Save();
        }

        private void InitializeMixins()
        {
            HarmonyInstance = HarmonyInstance.Create(Defaults.ReactorGameApiNamespace);
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
