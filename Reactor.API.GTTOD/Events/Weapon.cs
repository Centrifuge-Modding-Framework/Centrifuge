using Reactor.API.GTTOD.Events.Args;
using System;

namespace Reactor.API.GTTOD.Events
{
    public class Weapon
    {
        public static event EventHandler<MethodPreviewEventArgs<WeaponScript>> PreviewAwake;
        public static event EventHandler<ApiEventArgsBase<WeaponScript>> AwakeComplete;

        internal static void InvokePreviewAwake(MethodPreviewEventArgs<WeaponScript> args)
        {
            PreviewAwake?.Invoke(null, args);
        }

        internal static void InvokeAwakeComplete(ApiEventArgsBase<WeaponScript> args)
        {
            AwakeComplete?.Invoke(null, args);
        }
    }
}
