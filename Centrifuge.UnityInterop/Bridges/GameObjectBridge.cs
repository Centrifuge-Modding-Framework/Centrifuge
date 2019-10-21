﻿using System;
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

        public static void DontDestroyOnLoad(object gameObject)
        {
            var dontDestroy = ObjectType.GetMethod(
                "DontDestroyOnLoad",
                BindingFlags.Public | BindingFlags.Static
            );

            dontDestroy.Invoke(null, new[] { gameObject });
        }

        public static void SetActive(object gameObject, bool active)
        {
            var setActive = gameObject.GetType().GetMethod(
                "SetActive",
                BindingFlags.Public | BindingFlags.Instance
            );

            setActive.Invoke(gameObject, new object[] { active });
        }

        public static object AttachComponentTo(object gameObject, Type componentType)
        {
            var addComponent = gameObject.GetType().GetMethod(
                "AddComponent",
                new[] { componentType }
            );

            Debug.Assert(addComponent != null);
            return addComponent.Invoke(gameObject, new object[] { componentType });
        }
    }
}
