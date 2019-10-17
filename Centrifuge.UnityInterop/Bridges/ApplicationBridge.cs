using System;
using System.Reflection;

namespace Centrifuge.UnityInterop.Bridges
{
    public static class ApplicationBridge
    {
        public static Type ApplicationType => Kernel.FindTypeByFullName("UnityEngine.Application");
        public static Type LogCallbackType => Kernel.FindTypeByFullName("UnityEngine.Application.LogCallback");

        public static string UnityVersion => ApplicationType.GetProperty(
            "unityVersion",
            BindingFlags.Public | BindingFlags.Static
        ).GetGetMethod().Invoke(null, new object[] { }) as string;
    }
}
