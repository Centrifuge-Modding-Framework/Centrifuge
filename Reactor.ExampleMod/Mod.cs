using Reactor.API.Attributes;
using Reactor.API.Configuration;
using Reactor.API.DataModel;
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
        public const string ModID = "com.github.ciastex.ExampleMod";

        private Logger _logger;
        private Settings _settings;

        public void Awake()
        {
            _logger = new Logger("diagnostics");
            _settings = new Settings("config");

            _settings.GetOrCreate("TestKeyBind", "LeftControl+F1");

            if (_settings.Dirty)
            {
                _settings.Save();
            }

            Console.WriteLine("ExampleMod: Awake called.");
            _logger.Success("ExampleMod: logger seems to work.");
        }

        public void Start()
        {
            Console.WriteLine("ExampleMod: Start called.");
            _logger.Success("ExampleMod logger: Start called.");
        }

        public void Initialize(IManager manager)
        {
            manager.Hotkeys.Bind(new Hotkey(_settings.GetItem<string>("TestKeyBind")), () => { Console.WriteLine("REEEEEEEEEE"); });
            Console.WriteLine("ExampleMod: Initialize called.");

            var msg = new ModMessage(ModID, ModID, "YO FAT FUCK RESPOND");
            msg["test"] = "THIS BE A RESPONS";

            manager.Messenger.Send(msg);
        }

        [MessageHandler(ModID, "YO FAT FUCK RESPOND")]
        public static void HandleEchoMessage(ModMessage message)
        {
            if (message.Has("test"))
                Console.WriteLine(message["test"]);
        }
    }
}
