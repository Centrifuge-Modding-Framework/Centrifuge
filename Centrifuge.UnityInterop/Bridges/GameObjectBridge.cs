using System;
using System.Diagnostics;
using System.Reflection;

namespace Centrifuge.UnityInterop.Bridges
{
    public static class GameObjectBridge
    {
        public static Type GameObjectType => Kernel.FindTypeByFullName(Resources.UnityEngineGameObjectTypeName);
        public static Type ObjectType => Kernel.FindTypeByFullName(Resources.UnityEngineObjectTypeName);

        public static object CreateGameObject(string name)
            => Activator.CreateInstance(GameObjectType, new object[] { name });

        public static void DontDestroyOnLoad(object gameObj)
        {
            var dontDestroy = ObjectType.GetMethod(
                "DontDestroyOnLoad",
                BindingFlags.Public | BindingFlags.Static
            );

            dontDestroy.Invoke(null, new[] { gameObj });
        }

        public static void SetActive(object gameObj, bool active)
        {
            var setActive = gameObj.GetType().GetMethod(
                "SetActive",
                BindingFlags.Public | BindingFlags.Instance
            );

            // fixme: potential unnecessary boxing??
            setActive.Invoke(gameObj, new object[] { active });
        }

        public static object AttachComponentTo(object gameObject, Type componentType)
        {
            var addComponent = gameObject.GetType().GetMethod(
                "AddComponent",
                new[] { typeof(Type) },
                new[] { new ParameterModifier(1) }
            );

            Debug.Assert(addComponent != null);
            return addComponent.Invoke(gameObject, new object[] { componentType });
        }
    }
}
