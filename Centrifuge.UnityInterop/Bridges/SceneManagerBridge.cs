using System;
using System.Reflection;

namespace Centrifuge.UnityInterop.Bridges
{
    public class SceneManagerBridge
    {
        private static object PreviousSceneLoadedAttachTarget { get; set; }
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

        public static void AttachSceneLoadedEventHandler(object target)
        {
            Integrity.EnsureNotNull(target);
            Integrity.EnsureNull(SceneLoadedEventHandlerDelegate);

            var actualDelegateType = GenericDelegateType.MakeGenericType(new[] { SceneType, LoadSceneModeType });

            SceneLoadedEventHandlerDelegate = Delegate.CreateDelegate(
                actualDelegateType, target, Resources.Proxy.SceneLoadProxyMethodName, false, true
            );

            PreviousSceneLoadedAttachTarget = target;

            var ev = SceneManagerType.GetEvent(
                Resources.UnityEngine.SceneLoadedEventName,
                BindingFlags.Public | BindingFlags.Static
            );

            ev.AddEventHandler(null, SceneLoadedEventHandlerDelegate);
        }

        public static void DetachSceneLoadedEventHandler()
        {
            Integrity.EnsureNotNull(PreviousSceneLoadedAttachTarget);
            Integrity.EnsureNotNull(SceneLoadedEventHandlerDelegate);

            var ev = SceneManagerType.GetEvent(
                Resources.UnityEngine.SceneLoadedEventName,
                BindingFlags.Public | BindingFlags.Static
            );

            ev.RemoveEventHandler(PreviousSceneLoadedAttachTarget, SceneLoadedEventHandlerDelegate);
        }
    }
}
