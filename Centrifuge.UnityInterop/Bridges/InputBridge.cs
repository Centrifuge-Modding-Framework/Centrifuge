using System;
using System.Reflection;

namespace Centrifuge.UnityInterop.Bridges
{
    public static class InputBridge
    {
        public static Type InputType => Kernel.FindTypeByFullName("UnityEngine.Input");
        public static Type KeyCodeType => Kernel.FindTypeByFullName("UnityEngine.KeyCode");

        private static MethodInfo GetKey => InputType.GetMethod("GetKey", new[] { KeyCodeType });
        private static MethodInfo GetKeyDown => InputType.GetMethod("GetKeyDown", new[] { KeyCodeType });
        private static MethodInfo GetKeyUp => InputType.GetMethod("GetKeyUp", new[] { KeyCodeType });

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
