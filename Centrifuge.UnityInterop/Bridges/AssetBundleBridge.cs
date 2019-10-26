using System;
using System.Reflection;

namespace Centrifuge.UnityInterop.Bridges
{
    public static class AssetBundleBridge
    {
        public static Type AssetBundleType => Kernel.FindTypeByFullName(Resources.UnityEngine.AssetBundleTypeName);

        private static MethodInfo LoadFromFile => AssetBundleType.GetMethod(
            Resources.UnityEngine.AssetBundleLoadFromFileMethodName,
            new[] { typeof(string) }
        );

        public static object LoadFrom(string path)
        {
            return LoadFromFile.Invoke(null, new[] { path });
        }
    }
}
