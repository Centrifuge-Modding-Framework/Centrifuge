using Centrifuge.UnityInterop.Bridges;
using Reactor.API;
using Reactor.API.Attributes;
using Reactor.API.DataModel;
using Reactor.API.Exceptions;
using Reactor.API.Extensions;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using Reactor.Communication;
using Reactor.DataModel.Communication;
using Reactor.DataModel.Metadata;
using Reactor.DataModel.ModLoader;
using Reactor.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Reactor.Extensibility
{
    internal class ModLoader
    {
        private Manager Manager { get; }
        private Messenger Messenger { get; }

        private ModRegistry Registry { get; }
        private string SourceDirectory { get; }

        private Log Log => LogManager.GetForInternalAssembly();

        public ModLoader(Manager manager, string sourceDirectory, ModRegistry registry)
        {
            Manager = manager;
            Messenger = manager.Messenger as Messenger;

            SourceDirectory = sourceDirectory;
            Registry = registry;

            if (!Directory.Exists(SourceDirectory))
            {
                Log.Info($"Directory {SourceDirectory} does not exist. Creating.");
                Directory.CreateDirectory(SourceDirectory);
            }
        }

        public void Initialize()
        {
            var loadData = PrepareLoadData();
            LoadAllMods(loadData);

            // this warms up registry query cache as well
            var loadedMods = Registry.GetLoadedMods();

            Log.Info($"-- MOD LOADER INIT COMPLETE --");
            Log.Info($" >> {loadData.Count} manifest(s) parsed, {loadedMods.Count} mod(s) loaded.");
        }

        private List<LoadData> PrepareLoadData()
        {
            var modDirectories = Directory.GetDirectories(SourceDirectory);
            var modLoadDataList = new List<LoadData>();

            foreach (var rootPath in modDirectories)
            {
                var directoryName = Path.GetFileName(rootPath);

                Log.Info($"Found directory '{directoryName}'");
                var manifestPath = Path.Combine(rootPath, Defaults.ManifestFileName);

                if (!File.Exists(manifestPath))
                {
                    Log.Warning($"Skipping directory without manifest.");
                    continue;
                }

                ModManifest manifest;
                try
                {
                    manifest = ModManifest.FromFile(manifestPath);

                    var validationErrors = manifest.Validate();
                    if (validationErrors != 0)
                    {
                        var sb = new StringBuilder();

                        if (validationErrors.HasFlag(ManifestValidationFlags.MissingFriendlyName))
                        {
                            sb.AppendLine(" * Missing 'FriendlyName' property.");
                        }

                        if (validationErrors.HasFlag(ManifestValidationFlags.MissingModuleFileName))
                        {
                            sb.AppendLine(" * Missing 'ModuleFileName' property.");
                        }

                        Log.Error($"Refusing to load the mod. Manifest has the following issues:\n{sb.ToString()}");
                        continue;
                    }

                    if (manifest.SkipLoad)
                    {
                        Log.Warning("Skipping due to 'SkipLoad' property set to true.");
                        continue;
                    }

                    modLoadDataList.Add(
                        new LoadData
                        {
                            RootDirectory = rootPath,
                            Manifest = manifest
                        }
                    );
                }
                catch (ManifestReadException mre)
                {
                    Log.Error($"Manifest is invalid: {mre.Message}");
                    continue;
                }
            }

            modLoadDataList = modLoadDataList.OrderByDescending(x => x.Manifest.Priority).ToList();
            return modLoadDataList;
        }

        private void LoadAllMods(List<LoadData> loadData)
        {
            if (!Directory.Exists(SourceDirectory))
            {
                Log.Warning($"Mod repository '{SourceDirectory}' doesn't exist. Creating and skipping mod loading step.");
                Directory.CreateDirectory(SourceDirectory);
            }
            else
            {
                foreach (var dataObject in loadData)
                {
                    try
                    {
                        LoadMod(dataObject);
                    }
                    catch (TooManyEntryPointsException)
                    {
                        Log.Error("Mod assembly has more than one entry point defined.");
                    }
                    catch (MissingEntryPointException)
                    {
                        Log.Error("Mod assembly has no entry points defined.");
                    }
                    catch (InvalidModIdException e)
                    {
                        Log.Error($"Mod ID is invalid: {e.Message}");
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Failed to load mod '{dataObject.Manifest.FriendlyName}'.");
                        Log.Exception(e);
                    }
                }
            }

            Manager.OnInitFinished();
        }

        private void LoadMod(LoadData data)
        {
            var rootPath = data.RootDirectory;
            var manifest = data.Manifest;

            var targetModulePath = Path.Combine(rootPath, manifest.ModuleFileName);
            Log.Info($"Now trying to load '{targetModulePath}'");

            if (!File.Exists(targetModulePath))
            {
                Log.Error($"That was quick... Target DLL file does not exist.");
                return;
            }

            if (manifest.Dependencies != null && manifest.Dependencies.Length > 0)
            {
                if (!LoadDependenciesForMod(rootPath, manifest.Dependencies))
                {
                    Log.Error("Failed to load dependencies.");
                    return;
                }
            }

            Assembly modAssembly;
            try
            {
                modAssembly = Assembly.LoadFrom(targetModulePath);
            }
            catch (ReflectionTypeLoadException rtle)
            {
                Log.ReflectionTypeLoadException(rtle);
                return;
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return;
            }

            EnsureSingleEntryPoint(modAssembly);

            Type[] types;
            try
            {
                types = modAssembly.GetTypes();
            }
            catch (ReflectionTypeLoadException rtle)
            {
                Log.ReflectionTypeLoadException(rtle);
                return;
            }

            var entryPointType = FindEntryPointType(types);
            var entryPointInfo = GetEntryPointAttribute(entryPointType);

            EnsureModIdValid(entryPointInfo.ModID);

            var initMethodInfo = entryPointType.GetMethod(entryPointInfo.InitializerName, new Type[] { typeof(IManager) });
            if (initMethodInfo == null)
            {
                Log.Error($"Initializer method '{entryPointInfo.InitializerName}' accepting parameter of type 'IManager' not found.");
                return;
            }

            var messageHandlers = FindMessageHandlers(types);
            foreach (var messageHandler in messageHandlers)
            {
                Messenger.RegisterHandlerFor(messageHandler.ModID, messageHandler.MessageName, messageHandler.Method);
                Log.Info($"Registered message handler <{messageHandler.Method.Name}> for '{messageHandler.ModID}:{messageHandler.MessageName}'");
            }

            var modHost = new ModHost
            {
                Assembly = modAssembly,
                ModID = entryPointInfo.ModID,
                LoadData = data,
                Instance = null,
                GameObject = null
            };

            var dealingWithGameObject = MonoBehaviourBridge.MonoBehaviourType.IsAssignableFrom(entryPointType);
            if (dealingWithGameObject)
            {
                modHost.GameObject = GameObjectBridge.CreateGameObject(entryPointInfo.ModID);
                GameObjectBridge.DontDestroyOnLoad(modHost.GameObject);

                if (entryPointInfo.AwakeAfterInitialize)
                {
                    GameObjectBridge.SetActive(modHost.GameObject, false);
                }

                modHost.Instance = GameObjectBridge.AttachComponentTo(modHost.GameObject, entryPointType);
            }
            else
            {
                var instance = Activator.CreateInstance(entryPointType);
                modHost.Instance = instance;
            }

            Registry.RegisterMod(modHost);
            Log.Info("Loaded and registered successfully. Initializing.");

            var initializer = entryPointType.GetMethod(
                entryPointInfo.InitializerName,
                new Type[] { typeof(IManager) }
            );

            if (initializer != null)
            {
                initializer.Invoke(
                    modHost.Instance,
                    new object[] { Manager }
                );
            }
            else
            {
                Log.Error($"Failed to call initializer method '{entryPointInfo.InitializerName}' for Mod ID '{modHost.ModID}'. Seems like it doesn't exist.");
            }

            if (dealingWithGameObject && entryPointInfo.AwakeAfterInitialize)
                GameObjectBridge.SetActive(modHost.GameObject, true);

            Manager.OnModInitialized(modHost.ToExchangeableApiObject());
        }

        private bool LoadDependenciesForMod(string rootPath, string[] deps)
        {
            var baseDependencyDirPath = Path.Combine(rootPath, Defaults.PrivateDependencyDirectory);

            foreach (var dep in deps)
            {
                var targetDepPath = Path.Combine(baseDependencyDirPath, dep);

                try
                {
                    Log.Info($"Loading dependency assembly '{dep}'...");
                    Assembly.LoadFrom(targetDepPath);
                }
                catch (ReflectionTypeLoadException rtle)
                {
                    Log.ReflectionTypeLoadException(rtle);
                    return false;
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                    return false;
                }
            }

            return true;
        }

        private Type FindEntryPointType(Type[] types)
        {
            foreach (var type in types)
            {
                var attributes = type.GetCustomAttributes(typeof(ModEntryPointAttribute), true);

                if (attributes.Length == 1)
                    return type;
            }

            throw new MissingEntryPointException("This assembly has no entry points.");
        }

        private ModEntryPointAttribute GetEntryPointAttribute(Type type)
        {
            return type.GetCustomAttributes(typeof(ModEntryPointAttribute), true)[0] as ModEntryPointAttribute;
        }

        private List<MessageHandlerInvocationParameters> FindMessageHandlers(Type[] types)
        {
            var messageHandlerParameters = new List<MessageHandlerInvocationParameters>();

            foreach (var type in types)
            {
                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
                {
                    var attribs = method.GetCustomAttributes(typeof(MessageHandlerAttribute), true);

                    if (attribs.Length == 0)
                        continue;

                    var parameters = method.GetParameters();
                    if (parameters.Length == 1)
                    {
                        if (parameters[0].ParameterType != typeof(ModMessage))
                        {
                            Log.Warning($"Message handler '{method.Name}' has an invalid signature. Ignoring the method.");
                            continue;
                        }
                    }
                    else
                    {
                        Log.Warning($"Message handler '{method.Name}' has an invalid signature. Ignoring the method.");
                        continue;
                    }

                    var attrib = attribs[0] as MessageHandlerAttribute;
                    var messageHandlerParameter = new MessageHandlerInvocationParameters
                    {
                        Method = method,
                        ModID = attrib.SourceModID,
                        MessageName = attrib.MessageName
                    };

                    messageHandlerParameters.Add(messageHandlerParameter);
                }
            }

            return messageHandlerParameters;
        }

        private void EnsureSingleEntryPoint(Assembly assembly)
        {
            List<object> attribs = new List<object>();

            foreach (var t in assembly.GetExportedTypes())
            {
                attribs.AddRange(t.GetCustomAttributes(typeof(ModEntryPointAttribute), true));
            }

            if (attribs.Count == 0)
                throw new MissingEntryPointException("This assembly has no entry points.");

            if (attribs.Count > 1)
                throw new TooManyEntryPointsException("This assembly has more than one entry point defined.");
        }

        private void EnsureModIdValid(string modId)
        {
            if (string.IsNullOrEmpty(modId))
                throw new InvalidModIdException("Cannot be empty or null.");

            if (modId == "*")
                throw new InvalidModIdException("'*' is a reserved broadcast name - you have the entire UTF-8 for fuck's sake, use it.");
        }
    }
}
