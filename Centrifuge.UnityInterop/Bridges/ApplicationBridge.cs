using System;
using System.Reflection;

namespace Centrifuge.UnityInterop.Bridges
{
    public static class ApplicationBridge
    {
        public static Type ApplicationType => Kernel.FindTypeByFullName("UnityEngine.Application");

        public static string UnityVersion => ApplicationType.GetField(
            "unityVersion", 
            BindingFlags.Public | BindingFlags.Static
        ).GetValue(null) as string;
    }
}
