using System;
using System.Reflection;

namespace Centrifuge.UnityInterop.Bridges
{
    public static class SceneManagementBridge
    {
        public static Type SceneType => Kernel.FindTypeByFullName("UnityEngine.SceneManagement.Scene");
        public static Type SceneManagerType => Kernel.FindTypeByFullName("UnityEngine.SceneManagement.SceneManager");
        public static Type LoadSceneModeType => Kernel.FindTypeByFullName("UnityEngine.SceneManagement.LoadSceneMode");

        private static Delegate _onLoadDelegate;

        public static void AttachOnLoadEventHandler(object target)
        {
            if(_onLoadDelegate != null)
            {
                Console.WriteLine("SceneManagementBridge: tried to attach a delegate twice!");
                return;
            }

            var ev = SceneManagerType.GetEvent(
                "sceneLoaded",
                BindingFlags.Public | BindingFlags.Static
            );

            _onLoadDelegate = Delegate.CreateDelegate(ev.EventHandlerType, target, "OnLoadProxy", false, true);
            ev.AddEventHandler(target, _onLoadDelegate);
        }

        // Currently unused.
        public static void DetachOnLoadEventHandler(object target)
        {
            if (_onLoadDelegate == null)
            {
                Console.WriteLine("SceneManagementBridge: tried to detach a null delegate!");
                return;
            }

            var ev = SceneManagerType.GetEvent(
                "sceneLoaded",
                BindingFlags.Public | BindingFlags.Static
            );

            ev.RemoveEventHandler(target, _onLoadDelegate);
        }
    }
}
