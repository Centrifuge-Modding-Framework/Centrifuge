using Harmony;
using Reactor.API.Configuration;
using Reactor.API.GTTOD.Internal;
using System;
using System.Reflection;
using UnityEngine;
using Logger = Reactor.API.Logging.Logger;

namespace Reactor.API.GTTOD
{
    public class GameAPI : MonoBehaviour
    {
        private Settings Settings { get; set; }
        private Logger Logger { get; set; }

        private HarmonyInstance HarmonyInstance { get; set; }
        private Terminal Terminal { get; set; }

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Logger = new Logger("game_api");

            InitializeSettings();
            Terminal = new Terminal(Settings);

            try
            {
                InitializeMixins();
            }
            catch (Exception e)
            {
                Logger.Error("Failed to initialize Game API mix-ins. Mods will still be loaded, but may not function correctly.");
                Logger.ExceptionSilent(e);
            }
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

            Transpilers.WeaponScript.ApplyAll(HarmonyInstance);
        }
    }
}
