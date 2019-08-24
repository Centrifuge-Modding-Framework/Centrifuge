using Reactor.API.GTTOD.Events.Args;
using System;

namespace Reactor.API.GTTOD.Events
{
    public class Weapon
    {
        public static event EventHandler<MethodPreviewEventArgs<WeaponScript>> PreviewAwake;
        public static event EventHandler<TypeInstanceEventArgs<WeaponScript>> AwakeComplete;
        public static event EventHandler<WeaponFireEventArgs> ShotFired;

        internal static void InvokePreviewAwake(MethodPreviewEventArgs<WeaponScript> e)
            => PreviewAwake?.Invoke(null, e);

        internal static void InvokeAwakeComplete(TypeInstanceEventArgs<WeaponScript> e)
            => AwakeComplete?.Invoke(null, e);

        internal static void InvokeShotFiredPrimary(WeaponScript weapon)
            => ShotFired?.Invoke(null, new WeaponFireEventArgs(weapon.gameObject) { IsPrimaryFire = true });

        internal static void InvokeShotFiredSecondary(WeaponScript weapon)
            => ShotFired?.Invoke(null, new WeaponFireEventArgs(weapon.gameObject) { IsPrimaryFire = false });
    }
}

