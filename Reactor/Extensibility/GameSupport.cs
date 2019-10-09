using Reactor.API;
using Reactor.API.Attributes;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Logger = Reactor.API.Logging.Logger;

namespace Reactor.Extensibility
{
    internal class GameSupport
    {
        private Logger Logger { get; }

        public GameSupport()
        {
            Logger = new Logger(Defaults.GameSupportInitializerLogFileName);
        }

        public void Initialize()
        {
            Logger.Info("Trying to find a single game support library...");

            var gameSupportLibs = Directory.GetFiles(Defaults.CentrifugeRoot, Defaults.GameSupportLibraryFilePattern);

            if (gameSupportLibs.Length == 0)
            {
                Logger.Warning("Game support library not found, skipping this phase.");
                return;
            }

            if(gameSupportLibs.Length > 1)
            {
                Logger.Warning("More than one game support library detected, skipping this phase.");
                Logger.Warning("Remove redundant game support libraries and restart the game.");

                return;
            }

            Logger.Info("Single game support library found. Trying to initialize...");

            try
            {
                var asm = Assembly.LoadFrom(gameSupportLibs[0]);
                InitializeGameSupport(asm);

                Logger.Success("Game support initialized.");
            }
            catch (Exception e)
            {
                Logger.Error("Failed to initialize game support.");
                Logger.Exception(e);
            }
        }

        private void InitializeGameSupport(Assembly assy)
        {
            var types = assy.GetTypes();
            var decoratedType = types.FirstOrDefault(
                                    t => t.GetCustomAttributes(
                                        typeof(GameSupportLibraryEntryPointAttribute),
                                        false
                                    ).Length == 1
                                );

            if (decoratedType == null)
            {
                Logger.Error("The game support library is present, but doesn't contain a marked entry point.");
                return;
            }

            if (decoratedType.IsAssignableFrom(typeof(MonoBehaviour)))
            {
                Logger.Error("The game support library has a decorated entry point but it doesn't inherit from MonoBehaviour.");
                return;
            }

            if (!decoratedType.Attributes.HasFlag(TypeAttributes.Sealed))
            {
                Logger.Error("The game support library has an entry point but its class is not sealed.");
                return;
            }

            var attribute = decoratedType.GetCustomAttributes(
                typeof(GameSupportLibraryEntryPointAttribute), false
            ).First() as GameSupportLibraryEntryPointAttribute;

            Global.GameApiObject = new GameObject(attribute.LibraryID);
            Global.GameApiObject.AddComponent(decoratedType);
        }
    }
}
