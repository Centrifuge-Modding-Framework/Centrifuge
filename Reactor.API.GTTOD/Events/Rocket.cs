using Reactor.API.GTTOD.Events.Args;
using System;

namespace Reactor.API.GTTOD.Events
{
    public class Rocket
    {
        public static event EventHandler<MethodPreviewEventArgs<global::Rocket>> PreviewHailFire;
        public static event EventHandler<TypeInstanceEventArgs<global::Rocket>> HailFiring;

        public static event EventHandler<MethodPreviewEventArgs<global::Rocket>> PreviewDetonate;
        public static event EventHandler<TypeInstanceEventArgs<global::Rocket>> Detonating;
        public static event EventHandler<TypeInstanceEventArgs<global::Rocket>> Detonated;

        internal static void OnPreviewHailFire(global::Rocket rocket)
        {
            PreviewHailFire?.Invoke(
                null,
                new MethodPreviewEventArgs<global::Rocket>(rocket)
            );
        }

        internal static void OnHailFiring(global::Rocket rocket)
        {
            HailFiring?.Invoke(
                null, new TypeInstanceEventArgs<global::Rocket>(rocket)
            );
        }

        internal static void OnPreviewDetonate(global::Rocket rocket)
        {
            PreviewDetonate?.Invoke(
                null,
                new MethodPreviewEventArgs<global::Rocket>(rocket)
            );
        }

        internal static void OnDetonating(global::Rocket rocket)
        {
            Detonating?.Invoke(
                null,
                new TypeInstanceEventArgs<global::Rocket>(rocket)
            );
        }

        internal static void OnDetonated(global::Rocket rocket)
        {
            Detonated?.Invoke(
                null,
                new TypeInstanceEventArgs<global::Rocket>(rocket)
            );
        }
    }
}
