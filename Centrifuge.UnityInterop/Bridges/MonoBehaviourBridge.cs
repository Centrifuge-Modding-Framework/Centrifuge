using System;

namespace Centrifuge.UnityInterop.Bridges
{
    public static class MonoBehaviourBridge
    {
        public static Type MonoBehaviourType => Kernel.FindTypeByFullName(
            Resources.UnityEngine.MonoBehaviourTypeName,
            Resources.UnityEngine.AssemblyNameFilter
        );
    }
}
