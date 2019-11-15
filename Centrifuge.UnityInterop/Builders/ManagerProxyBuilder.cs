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
            ProxyAssemblyName = new AssemblyName(Resources.Proxy.AssemblyName);
            ProxyAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(ProxyAssemblyName, AssemblyBuilderAccess.Run);
            ProxyModuleBuilder = ProxyAssemblyBuilder.DefineDynamicModule(Resources.Proxy.ModuleName);
            ProxyTypeBuilder = ProxyModuleBuilder.DefineType(
                Resources.Proxy.ManagerTypeName,
                  TypeAttributes.Class |
                  TypeAttributes.Public |
                  TypeAttributes.AnsiClass |
                  TypeAttributes.BeforeFieldInit,
                MonoBehaviourBridge.MonoBehaviourType
            );

            BuildManagerField();
            BuildLoggerProxy();
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
                Resources.Proxy.ManagerFieldName,
                ReactorBridge.ReactorManagerType,
                FieldAttributes.Public
            );
        }

        private void BuildLoggerProxy()
        {
            var proxyMethod = ProxyTypeBuilder.DefineMethod(
                Resources.Proxy.LogProxyMethodName,
                    MethodAttributes.Public |
                    MethodAttributes.HideBySig,
                CallingConventions.HasThis,
                typeof(void),
                new[] { typeof(string), typeof(string), ApplicationBridge.LogTypeType }
            );

            var loggerMethod = ReactorBridge.ReactorUnityLogType.GetMethod(
                Resources.ReactorManager.UnityLogMethodName,
                new[] { typeof(string), typeof(string), typeof(int) }
            );

            var propertyGetMethod = ReactorBridge.ReactorManagerType.GetProperty(
                Resources.ReactorManager.UnityLogPropertyName,
                BindingFlags.Instance | BindingFlags.Public
            ).GetGetMethod();

            var ilGen = proxyMethod.GetILGenerator();

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, ProxyTypeBuilder.GetField(Resources.Proxy.ManagerFieldName));
            ilGen.Emit(OpCodes.Callvirt, propertyGetMethod);
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Ldarg_2);
            ilGen.Emit(OpCodes.Ldarg_3);
            ilGen.Emit(OpCodes.Callvirt, loggerMethod);
            ilGen.Emit(OpCodes.Ret);
        }

        private void BuildAwakeMethod()
        {
            var methodBuilder = ProxyTypeBuilder.DefineMethod(
                Resources.Proxy.AwakeMethodName,
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
                    Resources.UnityEngine.MonoBehaviourGameObjectFieldName,
                    BindingFlags.Public | BindingFlags.Instance
                ).GetGetMethod()
            );

            ilGen.Emit(
                OpCodes.Call,
                GameObjectBridge.ObjectType.GetMethod(
                    Resources.UnityEngine.ObjectDontDestroyOnLoadMethodName,
                    BindingFlags.Public | BindingFlags.Static
                )
            );

            // ApplicationBridge.AttachLoggingEventHandler(this);
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(
                OpCodes.Call,
                typeof(ApplicationBridge).GetMethod(
                    nameof(ApplicationBridge.AttachLoggingEventHandler),
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
                ProxyTypeBuilder.GetField(Resources.Proxy.ManagerFieldName)
            );

            ilGen.Emit(OpCodes.Ret);
        }

        private void BuildUpdateMethod()
        {
            var methodBuilder = ProxyTypeBuilder.DefineMethod(
                Resources.Proxy.UpdateMethodName,
                    MethodAttributes.Public |
                    MethodAttributes.HideBySig,
                CallingConventions.HasThis
            );

            var ilGen = methodBuilder.GetILGenerator();

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(
                OpCodes.Ldfld,
                ProxyTypeBuilder.GetField(Resources.Proxy.ManagerFieldName)
            );
            ilGen.Emit(
                OpCodes.Callvirt,
                ReactorBridge.ReactorManagerType.GetMethod(
                    Resources.ReactorManager.UpdateMethodName,
                    BindingFlags.Instance | BindingFlags.Public
                )
            );
            ilGen.Emit(OpCodes.Ret);
        }
    }
}
