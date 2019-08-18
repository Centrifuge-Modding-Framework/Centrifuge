using CommandTerminal;
using Reactor.API.Attributes;
using Reactor.API.Configuration;
using Reactor.API.DataModel;
using Reactor.API.GTTOD.Events.Args;
using Reactor.API.Input;
using Reactor.API.Interfaces.Systems;
using System;
using UnityEngine;
using Logger = Reactor.API.Logging.Logger;

namespace Reactor.ExampleMod
{
    [ModEntryPoint(ModID)]
    public class Mod : MonoBehaviour
    {
        public const string ModID = "com.github.ciastex/ExampleMod";

        private readonly Logger _logger = new Logger("diagnostics");
        private Settings _settings;

        public void Initialize(IManager manager)
        {
            _settings = new Settings("config");

            _settings.GetOrCreate("TestKeyBind", "LeftControl+F1");

            if (_settings.Dirty)
            {
                _settings.Save();
            }

            Terminal.Shell.AddCommand("examplemod_test", (args) =>
            {
                Terminal.Log("Looks like it works.");
            });

            manager.Hotkeys.Bind(new Hotkey(_settings.GetItem<string>("TestKeyBind")), () => { Console.WriteLine("REEEEEEEEEE"); });
            Console.WriteLine("ExampleMod: Initialize called.");

            var msg = new ModMessage(ModID, ModID, "YO FAT FUCK RESPOND");
            msg["test"] = "THIS BE A RESPONS";

            manager.Messenger.Send(msg);

            API.GTTOD.Events.Weapon.ShotFired += Weapon_ShotFired;
        }

        private void Weapon_ShotFired(object sender, WeaponFireEventArgs e)
        {
            var components = e.Instance.GetComponents<Component>();
            foreach (var comp in components)
            {
                _logger.Info($"{comp.GetType().Name}");
            }
        }

        public void Awake()
        {
            Console.WriteLine("ExampleMod: Awake called.");
            _logger.Success("ExampleMod: logger seems to work.");
        }

        public void Start()
        {
            Console.WriteLine("ExampleMod: Start called.");
            _logger.Success("ExampleMod logger: Start called.");
        }

        [MessageHandler(ModID, "YO FAT FUCK RESPOND")]
        public static void HandleEchoMessage(ModMessage message)
        {
            if (message.Has("test"))
                Console.WriteLine(message["test"]);
        }
    }
}
