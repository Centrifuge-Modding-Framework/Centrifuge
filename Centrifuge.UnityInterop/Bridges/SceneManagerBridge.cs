using System;
using System.Reflection;

namespace Centrifuge.UnityInterop.Bridges
{
    public class SceneManagerBridge
    {
        public static Delegate SceneLoadedEventHandlerDelegate { get; set; }
        
        public static Type SceneManagerType => Kernel.FindTypeByFullName(
            Resources.UnityEngine.SceneManagerTypeName,
            Resources.UnityEngine.AssemblyNameFilter
        );

        public static Type LoadSceneModeType => Kernel.FindTypeByFullName(
            Resources.UnityEngine.LoadSceneModeTypeName,
            Resources.UnityEngine.AssemblyNameFilter
        );

        public static Type SceneType => Kernel.FindTypeByFullName(
            Resources.UnityEngine.SceneTypeName,
            Resources.UnityEngine.AssemblyNameFilter
        );

        public static Type GenericDelegateType => Kernel.FindTypeByFullName(
            Resources.UnityEngine.UnityActionGenericTwoTypeName,
            Resources.UnityEngine.AssemblyNameFilter
        );

        public static void AttachSceneLoadedEventHandler(object target, string handlerMethodName)
        {
            Integrity.EnsureNotNull(target);
            Integrity.EnsureNull(SceneLoadedEventHandlerDelegate);

            SceneLoadedEventHandlerDelegate = Delegate.CreateDelegate(
                GenericDelegateType, target, handlerMethodName, false, true
            );

            var ev = SceneManagerType.GetEvent(
                Resources.UnityEngine.SceneLoadedEventName,
                BindingFlags.Public | BindingFlags.Static
            );

            ev.AddEventHandler(null, SceneLoadedEventHandlerDelegate);
        }

        public static void DetachSceneLoadedEventHandler(object target)
        {
            Integrity.EnsureNotNull(target);
            Integrity.EnsureNotNull(SceneLoadedEventHandlerDelegate);

            var ev = SceneManagerType.GetEvent(
                Resources.UnityEngine.SceneLoadedEventName,
                BindingFlags.Public | BindingFlags.Static
            );

            ev.RemoveEventHandler(target, SceneLoadedEventHandlerDelegate);
        }
    }
}
