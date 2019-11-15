using System;

namespace Centrifuge.UnityInterop.Bridges
{
    public static class ReactorBridge
    {
        public static Type ReactorManagerType = Kernel.FindTypeByFullName(Resources.ReactorManager.TypeName);
        public static Type ReactorUnityLogType = Kernel.FindTypeByFullName(Resources.ReactorManager.UnityLogTypeName);
    }
}
