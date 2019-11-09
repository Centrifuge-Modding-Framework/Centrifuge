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

        public string GameSupportID { get; private set; }

        public GameSupport()
        {
            Log = new Log(Defaults.GameSupportInitializerLogFileName);
            GameSupportID = string.Empty;
        }

        public void Initialize()
        {
            Log.Info("Trying to find a GSL...");

            var gameSupportLibs = Directory.GetFiles(Defaults.CentrifugeRoot, Defaults.GameSupportLibraryFilePattern);

            if (!gameSupportLibs.Any())
            {
                Log.Warning("Game support library not found, skipping this phase.");
                return;
            }

            if (gameSupportLibs.Length > 1)
            {
                Log.Warning("More than one game support library detected, skipping this phase.");
                Log.Warning("Remove redundant game support libraries and restart the game.");

                return;
            }

            Log.Info("GSL found. Trying to initialize...");

            try
            {
                var asm = Assembly.LoadFrom(gameSupportLibs[0]);
                InitializeGameSupport(asm);

                Log.Info("Game support library initialized.");
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
                Log.Error("The game support library is present, but doesn't contain a marked entry point.");
                return;
            }

            if (decoratedType.IsAssignableFrom(MonoBehaviourBridge.MonoBehaviourType))
            {
                Log.Error("The game support library has a decorated entry point but it doesn't inherit from MonoBehaviour.");
                return;
            }

            if (!decoratedType.Attributes.HasFlag(TypeAttributes.Sealed))
            {
                Log.Error("The game support library has an entry point but its class is not sealed.");
                return;
            }

            var attribute = decoratedType.GetCustomAttributes(
                typeof(GameSupportLibraryEntryPointAttribute),
                false
            ).First() as GameSupportLibraryEntryPointAttribute;

            Global.GameApiObject = GameObjectBridge.CreateGameObject(attribute.LibraryID);
            var component = GameObjectBridge.AttachComponentTo(Global.GameApiObject, decoratedType);

            if (component != null)
            {
                GameSupportID = attribute.LibraryID;
            }
            else
            {
                Log.Error("Game support library failed to initialize, for some reason your component was rejected by Unity Engine.\nLook in the output log of the game for more details.");
            }
        }
    }
}
