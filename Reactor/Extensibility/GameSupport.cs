using Centrifuge.UnityInterop.Bridges;
using Reactor.API;
using Reactor.API.Attributes;
using Reactor.API.Extensions;
using Reactor.API.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Reactor.Extensibility
{
    internal class GameSupport
    {
        private Log Log { get; }

        public GameSupport()
        {
            Log = new Log(Defaults.GameSupportInitializerLogFileName);

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
                    Log.TypeResolverFailure(rtle);
                }
                catch (Exception e)
                {
                    Log.Error("Failed to initialize game support.");
                    Log.Exception(e);
                }
            }
        }

        private bool InitializeGameSupport(Assembly assy)
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


            var gameObject = GameObjectBridge.CreateGameObject(attribute.LibraryID);
            var component = GameObjectBridge.AttachComponentTo(gameObject, decoratedType);

            if (component != null)
            {
                Global.GameApiObjects.Add(attribute.LibraryID, gameObject);
                return true;
            }
             
            Log.Error("Game support library failed to initialize, for some reason your component was rejected by Unity Engine.\nLook in the output log of the game for more details.");
            return false;
        }
    }
}
