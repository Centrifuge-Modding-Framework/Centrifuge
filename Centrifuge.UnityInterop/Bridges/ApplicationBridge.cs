using System;
using System.Reflection;

namespace Centrifuge.UnityInterop.Bridges
{
    public static class ApplicationBridge
    {
        public static Type ApplicationType => Kernel.FindTypeByFullName("UnityEngine.Application");
        public static Type LogCallbackType => Kernel.FindTypeByFullName("UnityEngine.Application+LogCallback");
        public static Type LogTypeType => Kernel.FindTypeByFullName("UnityEngine.LogType");

        public static string UnityVersion => ApplicationType.GetProperty(
            "unityVersion",
            BindingFlags.Public | BindingFlags.Static
        ).GetGetMethod().Invoke(null, new object[] { }) as string;

        public static bool IsSupportedUnityVersion()
        {
            var version = UnityVersion.Split('.');

            var major = int.Parse(version[0]);
            var minor = int.Parse(version[1]);

            if (major < 5 || (major == 5 && minor <= 1))
                return false;

            return true;
        }

        public static void AttachLoggingEventHandler(object target)
        {
            var ev = ApplicationType.GetEvent(
                "logMessageReceived",
                BindingFlags.Public | BindingFlags.Static
            );

            var d = Delegate.CreateDelegate(ev.EventHandlerType, target, "LogProxy", false, true);
            ev.AddEventHandler(null, d);
        }
    }
}
