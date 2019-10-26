using System;
using System.Reflection;

namespace Centrifuge.UnityInterop.Bridges
{
    public static class InputBridge
    {
        public static Type InputType => Kernel.FindTypeByFullName(Resources.UnityEngine.InputTypeName);
        public static Type KeyCodeType => Kernel.FindTypeByFullName(Resources.UnityEngine.KeyCodeTypeName);

        private static MethodInfo GetKey => InputType.GetMethod(
            Resources.UnityEngine.InputGetKeyMethodName,
            new[] { KeyCodeType }
        );

        private static MethodInfo GetKeyDown => InputType.GetMethod(
            Resources.UnityEngine.InputGetKeyDownMethodName,
            new[] { KeyCodeType }
        );

        private static MethodInfo GetKeyUp => InputType.GetMethod(
            Resources.UnityEngine.InputGetKeyUpMethodName,
            new[] { KeyCodeType }
        );

        public static bool IsKeyPressed(string key)
        {
            var keyCode = Enum.Parse(KeyCodeType, key);
            return (bool)GetKey.Invoke(null, new[] { keyCode });
        }

        public static bool IsKeyDown(string key)
        {
            var keyCode = Enum.Parse(KeyCodeType, key);
            return (bool)GetKeyDown.Invoke(null, new[] { keyCode });
        }

        public static bool IsKeyUp(string key)
        {
            var keyCode = Enum.Parse(KeyCodeType, key);
            return (bool)GetKeyUp.Invoke(null, new[] { keyCode });
        }
    }
}
