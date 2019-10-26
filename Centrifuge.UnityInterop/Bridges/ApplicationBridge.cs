using Centrifuge.UnityInterop.DataModel;
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

        public static UnityGeneration GetRunningUnityGeneration()
        {
            var version = UnityVersion.Split('.');

            var major = int.Parse(version[0]);

            if (major >= 5)
                return UnityGeneration.Unity5OrNewer;

            return UnityGeneration.Unity4OrOlder;
        }

        public static void AttachLoggingEventHandler(object target)
        {
            Integrity.EnsureNotNull(target);

            var d = Delegate.CreateDelegate(LogCallbackType, target, "LogProxy", false, true);

            var ev = ApplicationType.GetEvent(
                "logMessageReceived",
                BindingFlags.Public | BindingFlags.Static
            );

            ev.AddEventHandler(null, d);
        }
    }
}
