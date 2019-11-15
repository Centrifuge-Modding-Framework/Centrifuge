namespace Centrifuge.UnityInterop
{
    internal static class Resources
    {
        internal static class UnityEngine
        {
            internal const string ApplicationTypeName = "UnityEngine.Application";
            internal const string AssetBundleTypeName = "UnityEngine.AssetBundle";
            internal const string GameObjectTypeName = "UnityEngine.GameObject";
            internal const string InputTypeName = "UnityEngine.Input";
            internal const string KeyCodeTypeName = "UnityEngine.KeyCode";
            internal const string LoadSceneModeTypeName = "UnityEngine.SceneManagement.LoadSceneMode";
            internal const string LogCallbackTypeName = "UnityEngine.Application+LogCallback";
            internal const string LogTypeTypeName = "UnityEngine.LogType";
            internal const string MonoBehaviourTypeName = "UnityEngine.MonoBehaviour";
            internal const string ObjectTypeName = "UnityEngine.Object";
            internal const string SceneManagerTypeName = "UnityEngine.SceneManagement.SceneManager";
            internal const string SceneTypeName = "UnityEngine.SceneManagement.Scene";

            internal const string AssetBundleLoadFromFileMethodName = "LoadFromFile";
            internal const string GameObjectAddComponentMethodName = "AddComponent";
            internal const string GameObjectSetActiveMethodName = "SetActive";
            internal const string InputGetKeyMethodName = "GetKey";
            internal const string InputGetKeyDownMethodName = "GetKeyDown";
            internal const string InputGetKeyUpMethodName = "GetKeyUp";
            internal const string ObjectDontDestroyOnLoadMethodName = "DontDestroyOnLoad";

            internal const string MonoBehaviourGameObjectFieldName = "gameObject";
            internal const string ApplicationVersionPropertyName = "unityVersion";
            internal const string LogMessageReceivedEventName = "logMessageReceived";
        }

        internal static class Proxy
        {
            internal const string AssemblyName = "Centrifuge.UnityInterop.DynamicProxyAssembly";

            internal const string ManagerFieldName = "Manager";
            internal const string ManagerTypeName = "ProxyManager";
            internal const string ModuleName = "UnityProxyModule";

            internal const string AwakeMethodName = "Awake";
            internal const string LogProxyMethodName = "LogProxy";
            internal const string UpdateMethodName = "Update";
        }

        internal static class ReactorManager
        {
            public const string TypeName = "Reactor.Manager";
            public const string UnityLogTypeName = "Reactor.UnityLog";

            public const string UnityLogPropertyName = "UnityLog";
            public const string UnityLogMethodName = "LogUnityEngineMessage";
            public const string UpdateMethodName = "Update";
        }
    }
}
