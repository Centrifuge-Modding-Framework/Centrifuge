using Centrifuge.UnityInterop.Bridges;
using Reactor.API;
using Reactor.API.Attributes;
using Reactor.API.Extensions;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Reactor.Extensibility
{
    internal class GameSupport
    {
        internal static List<GameSupportHost> GSLs { get; private set; }

        private Log Log => LogManager.GetForInternalAssembly();
        private IManager Manager { get; }

        static GameSupport()
        {
            GSLs = new List<GameSupportHost>();
        }

        public GameSupport(IManager manager)
        {
            Manager = manager;

            if (!Directory.Exists(Defaults.ManagerGameSupportDirectory))
            {
                Log.Info("GSL directory not found. Creating.");
                Directory.CreateDirectory(Defaults.ManagerGameSupportDirectory);
            }
        }

        public void Initialize()
        {
            Log.Info("Looking for GSLs...");

            var gameSupportLibs = Directory.GetFiles(Defaults.ManagerGameSupportDirectory);

            if (!gameSupportLibs.Any())
            {
                Log.Info("No GSLs found. Skipping this phase.");
                return;
            }

            Log.Info("GSLs found. Trying to initialize...");

            foreach (var libPath in gameSupportLibs)
            {
                try
                {
                    var asm = Assembly.LoadFrom(libPath);

                    if (InitializeGameSupport(asm))
                        Log.Info($"GSL '{libPath}' initialized.");
                }
                catch (ReflectionTypeLoadException rtle)
                {
                    Log.ReflectionTypeLoadException(rtle);
                }
                catch (Exception e)
                {
                    Log.Error("Failed to initialize game support.");
                    Log.Exception(e);
                }
            }
        }

        public static bool IsGameSupportLibraryPresent(string id)
            => GSLs.Exists(h => h.ID == id);

        private bool InitializeGameSupport(Assembly assembly)
        {
            var types = assembly.GetTypes();
            var decoratedType = types.FirstOrDefault(
                                    t => t.GetCustomAttributes(
                                        typeof(GameSupportLibraryEntryPointAttribute),
                                        false
                                    ).Length == 1
                                );

            if (decoratedType == null)
            {
                Log.Error("The game support library is present, but doesn't contain a marked entry point.");
                return false;
            }

            if (decoratedType.IsAssignableFrom(MonoBehaviourBridge.MonoBehaviourType))
            {
                Log.Error("The game support library has a decorated entry point but it doesn't inherit from MonoBehaviour.");
                return false;
            }

            if (!decoratedType.Attributes.HasFlag(TypeAttributes.Sealed))
            {
                Log.Error("The game support library has an entry point but its class is not sealed.");
                return false;
            }


            var attribute = decoratedType.GetCustomAttributes(
                typeof(GameSupportLibraryEntryPointAttribute),
                false
            ).First() as GameSupportLibraryEntryPointAttribute;

            if (IsGameSupportLibraryPresent(attribute.LibraryID))
            {
                Log.Error("The game support library has a duplicate ID. Contact the GSL developer for assistance.");
                return false;
            }

            var gameObject = GameObjectBridge.CreateGameObject(attribute.LibraryID);

            if (attribute.AwakeAfterInitialize)
                GameObjectBridge.SetActive(gameObject, false);

            var component = GameObjectBridge.AttachComponentTo(gameObject, decoratedType);

            if (component != null)
            {
                var initializerMethod = decoratedType.GetMethod(
                    attribute.InitializerName,
                    new Type[] { typeof(IManager) }
                );

                if (initializerMethod != null)
                {
                    Log.Info($"Found initializer method '{attribute.InitializerName}' for {attribute.LibraryID}, calling.");
                    initializerMethod.Invoke(component, new object[] { Manager });
                }

                GameObjectBridge.SetActive(gameObject, true);

                var host = new GameSupportHost
                {
                    ID = attribute.LibraryID,
                    Assembly = assembly,
                    ComponentInstance = component,
                    UnityGameObject = gameObject
                };

                GSLs.Add(host);
                return true;
            }

            Log.Error("Game support library failed to initialize, for some reason your component was rejected by Unity Engine.\nLook in the output log of the game for more details.");
            return false;
        }
    }
}
