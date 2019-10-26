using System;
using System.Reflection;

namespace Centrifuge.UnityInterop.Bridges
{
    public static class GameObjectBridge
    {
        public static Type GameObjectType => Kernel.FindTypeByFullName(Resources.UnityEngine.GameObjectTypeName);
        public static Type ObjectType => Kernel.FindTypeByFullName(Resources.UnityEngine.ObjectTypeName);

        public static object CreateGameObject(string name)
            => Activator.CreateInstance(GameObjectType, new object[] { name });

        public static void DontDestroyOnLoad(object gameObject)
        {
            Integrity.EnsureNotNull(gameObject);
            Integrity.EnsureGameObject(gameObject);

            var dontDestroy = ObjectType.GetMethod(
                Resources.UnityEngine.ObjectDontDestroyOnLoadMethodName,
                BindingFlags.Public | BindingFlags.Static
            );

            dontDestroy.Invoke(null, new[] { gameObject });
        }

        public static void SetActive(object gameObject, bool active)
        {
            Integrity.EnsureNotNull(gameObject);
            Integrity.EnsureGameObject(gameObject);

            var setActive = GameObjectType.GetMethod(
                Resources.UnityEngine.GameObjectSetActiveMethodName,
                BindingFlags.Public | BindingFlags.Instance
            );

            setActive.Invoke(gameObject, new object[] { active });
        }

        public static object AttachComponentTo(object gameObject, Type componentType)
        {
            Integrity.EnsureNotNull(gameObject);
            Integrity.EnsureGameObject(gameObject);

            var addComponent = GameObjectType.GetMethod(
                Resources.UnityEngine.GameObjectAddComponentMethodName,
                new[] { typeof(Type) }
            );

            Integrity.EnsureNotNull(componentType);
            return addComponent.Invoke(gameObject, new object[] { componentType });
        }
    }
}
