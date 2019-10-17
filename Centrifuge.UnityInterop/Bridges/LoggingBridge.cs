using System;

namespace Centrifuge.UnityInterop.Bridges
{
    public static class LoggingBridge
    {
        public static Type UnityEngineLogTypeType => Kernel.FindTypeByFullName(Resources.UnityEngineLogTypeTypeName);
    }
}
