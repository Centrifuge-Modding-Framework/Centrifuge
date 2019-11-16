using System;

namespace Centrifuge.UnityInterop.Bridges
{
    public static class ReactorBridge
    {
        public static Type ReactorManagerType = Kernel.FindTypeByFullName(
            Resources.ReactorManager.TypeName,
            Resources.ReactorManager.AssemblyNameFilter
        );

        public static Type ReactorUnityLogType = Kernel.FindTypeByFullName(
            Resources.ReactorManager.UnityLogTypeName,
            Resources.ReactorManager.AssemblyNameFilter
        );
    }
}
