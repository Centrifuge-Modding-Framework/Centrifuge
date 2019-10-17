﻿namespace Centrifuge.UnityInterop
{
    public static class Resources
    {
        public const string UnityEngineObjectTypeName = "UnityEngine.Object";
        public const string UnityEngineGameObjectTypeName = "UnityEngine.GameObject";
        public const string UnityEngineMonoBehaviourTypeName = "UnityEngine.MonoBehaviour";
        public const string UnityEngineLogTypeTypeName = "UnityEngine.LogType";

        public const string UnityEngineSceneManagerTypeName = "UnityEngine.SceneManagement.SceneManager";

        public const string ProxyAssemblyName = "Centrifuge.UnityInterop.DynamicProxyAssembly";
        public const string ProxyManagerTypeName = "ProxyManager";

        public const string ReactorManagerTypeName = "Reactor.Manager";
        public const string ReactorManagerLogMethodName = "LogUnityEngineMessage";

        public const string ReactorGlobalTypeName = "Reactor.Global";
        public const string ReactorGlobalInterceptUnityLogsFieldName = "InterceptUnityLogs";
    }
}
