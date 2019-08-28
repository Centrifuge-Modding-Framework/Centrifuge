using CommandTerminal;
using Reactor.API.Attributes;
using Reactor.API.Configuration;
using Reactor.API.DataModel;
using Reactor.API.GTTOD;
using Reactor.API.GTTOD.Events.Args;
using Reactor.API.Input;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Storage;
using System;
using UnityEngine;
using Logger = Reactor.API.Logging.Logger;

namespace Reactor.ExampleMod
{
    [ModEntryPoint(ModID)]
    public class Mod : MonoBehaviour
    {
        public const string ModID = "com.github.ciastex/ExampleMod";

        private Texture2D _exampleImageTexture;
        private bool _showTexture;

        private readonly FileSystem _fileSystem = new FileSystem();
        private readonly Logger _logger = new Logger("diagnostics");
        private readonly Settings _settings = new Settings("config");

        public void Initialize(IManager manager)
        {
            _settings.GetOrCreate("TestKeyBind", "LeftControl+F1");

            if (_settings.Dirty)
            {
                _settings.Save();
            }

            Terminal.Shell.AddCommand("toggle_cringe", (args) =>
            {
                _showTexture = !_showTexture;
            });

            _exampleImageTexture = _fileSystem.LoadTexture("stoptalking.jpg");

            manager.Hotkeys.Bind(new Hotkey(_settings.GetItem<string>("TestKeyBind")), () => { Console.WriteLine("REEEEEEEEEE"); });
            Console.WriteLine("ExampleMod: Initialize called.");

            var msg = new ModMessage(ModID, ModID, "YO FAT FUCK RESPOND");
            msg["test"] = "THIS BE A RESPONS";

            manager.Messenger.Send(msg);

            API.GTTOD.Events.Weapon.ShotFired += Weapon_ShotFired;
            API.GTTOD.Events.Game.GameModeStarted += Game_GameModeStarted;
            API.GTTOD.Events.Game.PauseMenuOpened += Game_PauseMenuOpened;
            API.GTTOD.Events.Game.PauseMenuClosed += Game_PauseMenuClosed;
            API.GTTOD.Events.Player.Died += Player_Died;
            EnemyChatter.AttackMessages.Add("REEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE!");
            EnemyChatter.AttackMessages.Add("I AM HERE TO CHEW BUBBLE ASS AND KICK GUM!");
            EnemyChatter.AttackMessages.Add("I GOT BALLS OF STEEL!");
            EnemyChatter.DeathMessages.Add("HOW DARE YOU KILL MY DEREK FRIEND");
            EnemyChatter.JokeMessages.Add("OwO wots dis?!");
            EnemyChatter.ReactionMessages.Add("OH GOD OH FUCK OH GOD OH FUCK");
            EnemyChatter.ReloadMessages.Add("SHIT I'M OUT AAAAAAAAAAAAAAAAAAAAA");
            EnemyChatter.AngryMessages.Add("NOW I'M REALLY PISSED OFF");
            EnemyChatter.AngryMessages.Add("CM'ERE YOU LIL' SHIT");

            API.GTTOD.Events.EnemyNPC.InfantryDied += EnemyNPC_InfantryDied;
        }

        private void Player_Died(object sender, EventArgs e)
        {
            _logger.Info("Lol, dead scrub.");
        }

        private void Game_PauseMenuOpened(object sender, EventArgs e)
        {
            _logger.Info("Pause menu opened.");
        }

        private void Game_PauseMenuClosed(object sender, EventArgs e)
        {
            _logger.Info("Pause menu closed.");
        }

        private void Game_GameModeStarted(object sender, EventArgs e)
        {
            _logger.Info("Game mode started!");
        }

        private void EnemyNPC_InfantryDied(object sender, EnemyDeathEventArgs e)
        {
            _logger.Info($"Derek died: {e.Enemy.transform.position}");
        }

        private void Weapon_ShotFired(object sender, WeaponFireEventArgs e)
        {
            var component = e.Instance.GetComponent<WeaponScript>();
            component.CurrentAmmo++;
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

        public void OnGUI()
        {
            if (_showTexture)
            {
                GUI.DrawTexture(
                    new Rect(60, 240, _exampleImageTexture.width / 4, _exampleImageTexture.height / 4),
                    _exampleImageTexture,
                    ScaleMode.ScaleToFit
                );
            }
        }

        [MessageHandler(ModID, "YO FAT FUCK RESPOND")]
        public static void HandleEchoMessage(ModMessage message)
        {
            if (message.Has("test"))
                Console.WriteLine(message["test"]);
        }
    }
}
