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
