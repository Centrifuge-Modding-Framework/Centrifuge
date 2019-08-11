using Reactor.API.Attributes;
using Reactor.API.Configuration;
using Reactor.API.Input;
using Reactor.API.Interfaces.Systems;
using System;
using UnityEngine;
using Logger = Reactor.API.Logging.Logger;

namespace Reactor.ExampleMod
{
    [ModEntryPoint("com.github.ciastex.ExampleMod")]
    public class Mod : MonoBehaviour
    {
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

            throw new Exception("dicks");
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
        }
    }
}
