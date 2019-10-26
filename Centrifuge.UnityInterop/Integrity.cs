using Centrifuge.UnityInterop.Bridges;
using System;

namespace Centrifuge.UnityInterop
{
    internal static class Integrity
    {
        internal static void EnsureGameObject(object gameObject)
        {
            var objType = gameObject.GetType();

            if (!GameObjectBridge.GameObjectType.IsAssignableFrom(objType))
                throw new InvalidOperationException("The object you prodvided is not a Unity GameObject.");
        }

        internal static void EnsureNotNull(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("The object you provided is null.");
        }
    }
}
