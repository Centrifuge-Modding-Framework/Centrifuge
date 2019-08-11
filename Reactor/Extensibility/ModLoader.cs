using Reactor.API;
using Reactor.API.Attributes;
using Reactor.API.DataModel;
using Reactor.API.Exceptions;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using Reactor.Communication;
using Reactor.DataModel;
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

        private Logger Log { get; }

        public ModLoader(Manager manager, string sourceDirectory, ModRegistry registry)
        {
            Manager = manager;
            Messenger = manager.Messenger as Messenger;

            SourceDirectory = sourceDirectory;
            Registry = registry;

            Log = new Logger(Defaults.ModLoaderLogFileName);
        }

        public void Init()
        {
            var data = PrepareLoadData();
            LoadAllMods(data);
        }

        private List<LoadData> PrepareLoadData()
        {
            var modDirectories = Directory.GetDirectories(SourceDirectory);
            var modLoadDataList = new List<LoadData>();

            foreach (var rootPath in modDirectories)
            {
                Log.Info($"Now processing manifest: {rootPath}");
                var manifestPath = Path.Combine(rootPath, Defaults.ManifestFileName);

                if (!File.Exists(manifestPath))
                {
                    Log.Warning($" Skipping directory without manifest.");
                    continue;
                }

                ModManifest manifest;
                try
                {
                    manifest = ModManifest.FromFile(manifestPath);
                    if (manifest == null)
                    {
                        Log.Error(" ModManifest.FromFile(string) has failed to return a non-null value.");
                        continue;
                    }

                    var validationErrors = manifest.Validate();

                    if (validationErrors != 0)
                    {
                        var sb = new StringBuilder();

                        if (validationErrors.HasFlag(ManifestValidationFlags.MissingFriendlyName))
                        {
                            sb.AppendLine("  - Missing 'FriendlyName' property.");
                        }

                        if (validationErrors.HasFlag(ManifestValidationFlags.MissingModuleFileName))
                        {
                            sb.AppendLine("  - Missing 'ModuleFileName' property.");
                        }

                        Log.Error($" Refusing to load the mod. Manifest has the following issues:\n{sb.ToString()}");
                        continue;
                    }

                    if (manifest.SkipLoad)
                    {
                        Log.Warning(" Skipping due to 'SkipLoad' property set to true.");
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
                catch (MetadataReadException mre)
                {
                    Log.Exception(mre);
                    continue;
                }
            }

            modLoadDataList = modLoadDataList.OrderByDescending(x => x.Manifest.Priority).ToList();
            return modLoadDataList;
        }

        private void LoadAllMods(List<LoadData> loadData)
        {
            foreach (var dataObject in loadData)
                LoadMod(dataObject);

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
                Log.Error($" That was quick... Target DLL file does not exist.");
                return;
            }

            if (manifest.Dependencies != null && manifest.Dependencies.Length > 0)
            {
                try
                {
                    LoadDependenciesForMod(rootPath, manifest.Dependencies);
                }
                catch (Exception e)
                {
                    Log.Error(" Failed to load dependencies, detailed exception has been logged to the mod loader log file.");
                    Log.ExceptionSilent(e);

                    return;
                }
            }

            Assembly modAssembly;
            try
            {
                modAssembly = Assembly.LoadFrom(targetModulePath);
            }
            catch (Exception e)
            {
                Log.Error(" Assembly.LoadFrom failed. Detailed exception has been logged to the mod loader log file.");
                Log.ExceptionSilent(e);

                return;
            }

            try
            {
                EnsureSingleEntryPoint(modAssembly);
            }
            catch (TooManyEntryPointsException)
            {
                Log.Error(" Mod assembly has more than one entry point defined.");
                return;
            }
            catch (MissingEntryPointException)
            {
                Log.Error(" Mod assembly has no entry points defined.");
                return;
            }

            Type[] types;
            try
            {
                types = modAssembly.GetTypes();
            }
            catch (Exception e)
            {
                Log.Error(" modAssembly.GetTypes() failed. Detailed exception has been logged to the mod loader log file.");
                Log.ExceptionSilent(e);

                return;
            }

            var entryPointType = FindEntryPointType(types);
            var entryPointInfo = GetEntryPointAttribute(entryPointType);

            try
            {
                EnsureModIdValid(entryPointInfo.ModID);
            }
            catch (ArgumentException e)
            {
                Log.Error($" Mod ID invalid: {e.Message}");
                return;
            }
            catch (Exception e)
            {
                Log.Error($" {e.Message}");
                return;
            }

            var initMethodInfo = entryPointType.GetMethod(entryPointInfo.InitializerName, new Type[] { typeof(IManager) });
            if (initMethodInfo == null)
            {
                Log.Error($" Initializer method '{entryPointInfo.InitializerName}' accepting parameter of type 'IManager' not found.");
                return;
            }

            var messageHandlers = FindMessageHandlers(types);
            foreach (var messageHandler in messageHandlers)
            {
                Messenger.RegisterHandlerFor(messageHandler.ModID, messageHandler.MessageName, messageHandler.Method);
                Log.Success($" Registered message handler '{messageHandler.ModID}:{messageHandler.MessageName}' <{messageHandler.Method.Name}>");
            }

            var modHost = new ModHost
            {
                ModID = entryPointInfo.ModID,
                LoadData = data,
                Instance = null,
                GameObject = null
            };

            if (typeof(UnityEngine.MonoBehaviour).IsAssignableFrom(entryPointType))
            {
                modHost.GameObject = new UnityEngine.GameObject(entryPointInfo.ModID);
                UnityEngine.Object.DontDestroyOnLoad(modHost.GameObject);

                modHost.Instance = modHost.GameObject.AddComponent(entryPointType);
            }
            else
            {
                var instance = Activator.CreateInstance(entryPointType);
                modHost.Instance = instance;
            }

            entryPointType.GetMethod(
                entryPointInfo.InitializerName,
                new Type[] { typeof(IManager) }
            ).Invoke(
                modHost.Instance,
                new object[] { Manager }
            );

            Registry.RegisterMod(modHost);
            Log.Success(" Loaded successfully.");

            Manager.OnModInitialized(modHost.ToExchangeableApiObject());
        }

        private void LoadDependenciesForMod(string rootPath, string[] deps)
        {
            var baseDependencyDirPath = Path.Combine(rootPath, Defaults.PrivateDependencyDirectory);

            foreach (var dep in deps)
            {
                var targetDepPath = Path.Combine(baseDependencyDirPath, dep);

                if (!File.Exists(targetDepPath))
                    throw new FileNotFoundException("Declared private dependency file not found.", targetDepPath);

                try
                {
                    Assembly.LoadFrom(targetDepPath);
                }
                catch
                {
                    throw;
                }
            }
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
                throw new ArgumentException("Mod ID cannot be empty nor null.");
        }
    }
}
