using Centrifuge.UnityInterop.Bridges;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Centrifuge.UnityInterop.Builders
{
    public class ManagerProxyBuilder
    {
        private AssemblyName ProxyAssemblyName { get; }
        private AssemblyBuilder ProxyAssemblyBuilder { get; }
        private ModuleBuilder ProxyModuleBuilder { get; }
        private TypeBuilder ProxyTypeBuilder { get; }

        public ManagerProxyBuilder()
        {
            ProxyAssemblyName = new AssemblyName(Resources.ProxyAssemblyName);
            ProxyAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(ProxyAssemblyName, AssemblyBuilderAccess.Run);
            ProxyModuleBuilder = ProxyAssemblyBuilder.DefineDynamicModule("UnityProxyModule");
            ProxyTypeBuilder = ProxyModuleBuilder.DefineType(
                Resources.ProxyManagerTypeName,
                  TypeAttributes.Class |
                  TypeAttributes.Public |
                  TypeAttributes.AnsiClass |
                  TypeAttributes.BeforeFieldInit,
                MonoBehaviourBridge.MonoBehaviourType
            );

            BuildManagerField();
            BuildLogProxy();

            BuildAwakeMethod();
            BuildUpdateMethod();
        }

        public Type Build()
        {
            return ProxyTypeBuilder.CreateType();
        }

        private void BuildManagerField()
        {
            ProxyTypeBuilder.DefineField(
                "Manager",
                ReactorBridge.ReactorManagerType,
                FieldAttributes.Public
            );
        }

        private void BuildLogProxy()
        {
            var methodBuilder = ProxyTypeBuilder.DefineMethod(
                "LogProxy",
                    MethodAttributes.Public |
                    MethodAttributes.HideBySig,
                CallingConventions.HasThis,
                typeof(void),
                new[] { typeof(string), typeof(string), LoggingBridge.UnityEngineLogTypeType }
            );

            var ilGen = methodBuilder.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(
                OpCodes.Ldfld,
                ProxyTypeBuilder.GetField("Manager")
            );
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Ldarg_2);
            ilGen.Emit(OpCodes.Ldarg_3);

            ilGen.Emit(
                OpCodes.Callvirt,
                ReactorBridge.ReactorManagerType.GetMethod(
                    Resources.ReactorManagerLogMethodName,
                    BindingFlags.Public | BindingFlags.Instance
                )
            );

            ilGen.Emit(OpCodes.Ret);
        }

        private void BuildAwakeMethod()
        {
            var methodBuilder = ProxyTypeBuilder.DefineMethod(
                "Awake",
                    MethodAttributes.Public |
                    MethodAttributes.HideBySig,
                CallingConventions.HasThis
            );

            var ilGen = methodBuilder.GetILGenerator();

            // UnityEngine.DontDestroyOnLoad
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(
                OpCodes.Call,
                MonoBehaviourBridge.MonoBehaviourType.GetProperty(
                    "gameObject",
                    BindingFlags.Public | BindingFlags.Instance
                ).GetGetMethod()
            );

            ilGen.Emit(
                OpCodes.Call,
                GameObjectBridge.ObjectType.GetMethod(
                    "DontDestroyOnLoad",
                    BindingFlags.Public | BindingFlags.Static
                )
            );
            // -------------------------------------------

            // Manager = new Manager();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(
                OpCodes.Newobj,
                ReactorBridge.ReactorManagerType.GetConstructor(new Type[] { })
            );

            ilGen.Emit(
                OpCodes.Stfld,
                ProxyTypeBuilder.GetField("Manager")
            );

            // if(Manager.InterceptUnityLogs)
            //   UnityEngine.Application.logMessageReceived += LogProxy;
            // 
            /*ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(
                OpCodes.Ldfld,
                ProxyTypeBuilder.GetField("Manager")
            );
            ilGen.Emit(
                OpCodes.Ldsfld,
                ReactorBridge.ReactorGlobalType.GetField(
                    Resources.ReactorGlobalInterceptUnityLogsFieldName,
                    BindingFlags.Public | BindingFlags.Static
                )
            );

            var retLabel = ilGen.DefineLabel();
            ilGen.Emit(OpCodes.Brfalse_S, retLabel);

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(
                OpCodes.Ldftn,
                ProxyTypeBuilder.GetMethod(
                    "LogProxy",
                    BindingFlags.Public | BindingFlags.Instance
                )
            );
            
            ilGen.Emit(
                OpCodes.Newobj,
                ApplicationBridge.LogCallbackType.GetConstructor(new[] {
                     typeof(object),
                     typeof(IntPtr)
                })
            );

            ilGen.Emit(
                OpCodes.Call,
                ApplicationBridge.ApplicationType.GetMethod(
                    "add_logMessageReceived",
                    BindingFlags.Public | BindingFlags.Static
                )
            );

            ilGen.MarkLabel(retLabel);*/
            ilGen.Emit(OpCodes.Ret);
        }

        private void BuildUpdateMethod()
        {
            var methodBuilder = ProxyTypeBuilder.DefineMethod(
                "Update",
                    MethodAttributes.Public |
                    MethodAttributes.HideBySig,
                CallingConventions.HasThis
            );

            var ilGen = methodBuilder.GetILGenerator();

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(
                OpCodes.Ldfld,
                ProxyTypeBuilder.GetField("Manager")
            );
            ilGen.Emit(
                OpCodes.Callvirt,
                ReactorBridge.ReactorManagerType.GetMethod(
                    "Update",
                    BindingFlags.Instance | BindingFlags.Public
                )
            );
            ilGen.Emit(OpCodes.Ret);
        }
    }
}
