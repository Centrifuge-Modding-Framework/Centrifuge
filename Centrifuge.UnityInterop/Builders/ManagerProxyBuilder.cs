using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Centrifuge.UnityInterop.Bridges;
using Mono.Cecil;
using Mono.Cecil.Cil;
using FieldAttributes = Mono.Cecil.FieldAttributes;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using ParameterAttributes = Mono.Cecil.ParameterAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;

namespace Centrifuge.UnityInterop.Builders
{
    public class ManagerProxyBuilder
    {
        private FieldDefinition ManagerFieldDefinition { get; }

        private AssemblyNameDefinition ProxyAssemblyNameDefinition { get; }
        private AssemblyDefinition ProxyAssemblyDefinition { get; }
        private ModuleDefinition ProxyMainModule { get; }
        private TypeDefinition ProxyTypeDefinition { get; }

        public ManagerProxyBuilder()
        {
            ProxyAssemblyNameDefinition = new AssemblyNameDefinition(Resources.Proxy.AssemblyName, new Version(1, 0));

            ProxyAssemblyDefinition =
                AssemblyDefinition.CreateAssembly(ProxyAssemblyNameDefinition, Resources.Proxy.AssemblyName,
                    ModuleKind.Dll);
            ProxyMainModule = ProxyAssemblyDefinition.MainModule;
            ProxyMainModule.RuntimeVersion = ".NETStandard,Version=v2.0";

            ProxyTypeDefinition = new TypeDefinition(
                Resources.Proxy.AssemblyName,
                Resources.Proxy.ManagerTypeName,
                TypeAttributes.Class |
                TypeAttributes.Public |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit,
                ProxyMainModule.ImportReference(MonoBehaviourBridge.MonoBehaviourType)
            );

            ManagerFieldDefinition = BuildManagerField();
            BuildConstructor();
            BuildLoggerProxy();
            BuildSceneLoadProxy();
            BuildAwakeMethod();
            BuildUpdateMethod();

            ProxyMainModule.Types.Add(ProxyTypeDefinition);
        }

        public Type WriteDynamicAssemblyAndLoadProxyType()
        {
            var asmLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var targetAsmPath = Path.Combine(asmLocation!, Resources.Proxy.AssemblyFileName);

            if (File.Exists(targetAsmPath))
            {
                File.Delete(targetAsmPath);
            }

            ProxyAssemblyDefinition.Write(targetAsmPath);

            var asm = Assembly.LoadFrom(targetAsmPath);
            return asm.GetTypes().First();
        }

        private FieldDefinition BuildManagerField()
        {
            var def = new FieldDefinition(
                Resources.Proxy.ManagerFieldName,
                FieldAttributes.Public,
                ProxyMainModule.ImportReference(ReactorBridge.ReactorManagerType));

            ProxyTypeDefinition.Fields.Add(def);

            return def;
        }

        private void BuildConstructor()
        {
            var methodDef = new MethodDefinition(
                ".ctor",
                MethodAttributes.Public |
                MethodAttributes.HideBySig |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName,
                ProxyMainModule.ImportReference(typeof(void))
            );

            var ilGen = methodDef.Body.GetILProcessor();
            // Manager = new Manager();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(
                OpCodes.Newobj,
                ProxyMainModule.ImportReference(ReactorBridge.ReactorManagerType.GetConstructor(new Type[] { }))
            );

            ilGen.Emit(
                OpCodes.Stfld,
                ManagerFieldDefinition
            );

            ilGen.Emit(OpCodes.Ret);
            ProxyTypeDefinition.Methods.Add(methodDef);
        }

        private void BuildLoggerProxy()
        {
            var methodDef = new MethodDefinition(
                Resources.Proxy.LogProxyMethodName,
                MethodAttributes.Public |
                MethodAttributes.HideBySig,
                ProxyMainModule.ImportReference(typeof(void)));

            methodDef.CallingConvention = MethodCallingConvention.Default;
            methodDef.Parameters.Add(
                new ParameterDefinition("msg", ParameterAttributes.None,
                    ProxyMainModule.ImportReference(typeof(string))));

            methodDef.Parameters.Add(
                new ParameterDefinition("state", ParameterAttributes.None,
                    ProxyMainModule.ImportReference(typeof(string))));

            methodDef.Parameters.Add(
                new ParameterDefinition("logType", ParameterAttributes.None,
                    ProxyMainModule.ImportReference(ApplicationBridge.LogTypeType)));


            var loggerMethod = ProxyMainModule.ImportReference(ReactorBridge.ReactorUnityLogType.GetMethod(
                Resources.ReactorManager.UnityLogMethodName,
                new[] {typeof(string), typeof(string), typeof(int)}
            ));

            var propertyGetMethod = ProxyMainModule.ImportReference(ReactorBridge.ReactorManagerType.GetProperty(
                Resources.ReactorManager.UnityLogPropertyName,
                BindingFlags.Instance | BindingFlags.Public
            ).GetGetMethod());

            var ilGen = methodDef.Body.GetILProcessor();

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, ManagerFieldDefinition);
            ilGen.Emit(OpCodes.Callvirt, propertyGetMethod);
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Ldarg_2);
            ilGen.Emit(OpCodes.Ldarg_3);
            ilGen.Emit(OpCodes.Callvirt, loggerMethod);
            ilGen.Emit(OpCodes.Ret);

            ProxyTypeDefinition.Methods.Add(methodDef);
        }

        private void BuildSceneLoadProxy()
        {
            var methodDef = new MethodDefinition(
                Resources.Proxy.SceneLoadProxyMethodName,
                MethodAttributes.Public |
                MethodAttributes.HideBySig,
                ProxyMainModule.ImportReference(typeof(void)));

            methodDef.Parameters.Add(
                new ParameterDefinition(ProxyMainModule.ImportReference(SceneManagerBridge.SceneType))
            );

            methodDef.Parameters.Add(
                new ParameterDefinition(ProxyMainModule.ImportReference(SceneManagerBridge.LoadSceneModeType))
            );

            var assetLoadHookMethod = ProxyMainModule.ImportReference(ReactorBridge.ReactorManagerType.GetMethod(
                Resources.ReactorManager.CallAssetLoadHooksMethodName,
                BindingFlags.Instance | BindingFlags.Public
            ));

            var detachSceneLoadHandlerMethod = ProxyMainModule.ImportReference(
                typeof(SceneManagerBridge).GetMethod(
                    nameof(SceneManagerBridge.DetachSceneLoadedEventHandler),
                    BindingFlags.Public | BindingFlags.Static
                )
            );

            var ilGen = methodDef.Body.GetILProcessor();

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, ManagerFieldDefinition);
            ilGen.Emit(OpCodes.Callvirt, assetLoadHookMethod);
            ilGen.Emit(OpCodes.Callvirt, detachSceneLoadHandlerMethod);
            ilGen.Emit(OpCodes.Ret);

            ProxyTypeDefinition.Methods.Add(methodDef);
        }

        private void BuildAwakeMethod()
        {
            var methodDef = new MethodDefinition(
                Resources.Proxy.AwakeMethodName,
                MethodAttributes.Public |
                MethodAttributes.HideBySig,
                ProxyMainModule.ImportReference(typeof(void)));

            var ilGen = methodDef.Body.GetILProcessor();

            // UnityEngine.DontDestroyOnLoad
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(
                OpCodes.Call,
                ProxyMainModule.ImportReference(MonoBehaviourBridge.MonoBehaviourType.GetProperty(
                    Resources.UnityEngine.MonoBehaviourGameObjectFieldName,
                    BindingFlags.Public | BindingFlags.Instance
                ).GetGetMethod())
            );

            ilGen.Emit(
                OpCodes.Call,
                ProxyMainModule.ImportReference(GameObjectBridge.ObjectType.GetMethod(
                    Resources.UnityEngine.ObjectDontDestroyOnLoadMethodName,
                    BindingFlags.Public | BindingFlags.Static
                ))
            );

            // ApplicationBridge.AttachLoggingEventHandler(this);
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(
                OpCodes.Call,
                ProxyMainModule.ImportReference(typeof(ApplicationBridge).GetMethod(
                    nameof(ApplicationBridge.AttachLoggingEventHandler),
                    BindingFlags.Public | BindingFlags.Static
                ))
            );

            // SceneManagerBridge.AttachSceneLoadedEventHandler(this);
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(
                OpCodes.Call,
                ProxyMainModule.ImportReference(typeof(SceneManagerBridge).GetMethod(
                    nameof(SceneManagerBridge.AttachSceneLoadedEventHandler),
                    BindingFlags.Public | BindingFlags.Static
                ))
            );
            ilGen.Emit(OpCodes.Ret);

            ProxyTypeDefinition.Methods.Add(methodDef);
        }

        private void BuildUpdateMethod()
        {
            var methodDef = new MethodDefinition(
                Resources.Proxy.UpdateMethodName,
                MethodAttributes.Public |
                MethodAttributes.HideBySig,
                ProxyMainModule.ImportReference(typeof(void))
            );

            var ilGen = methodDef.Body.GetILProcessor();

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(
                OpCodes.Ldfld,
                ManagerFieldDefinition
            );
            ilGen.Emit(
                OpCodes.Callvirt,
                ProxyMainModule.ImportReference(ReactorBridge.ReactorManagerType.GetMethod(
                    Resources.ReactorManager.UpdateMethodName,
                    BindingFlags.Instance | BindingFlags.Public
                ))
            );
            ilGen.Emit(OpCodes.Ret);

            ProxyTypeDefinition.Methods.Add(methodDef);
        }
    }
}