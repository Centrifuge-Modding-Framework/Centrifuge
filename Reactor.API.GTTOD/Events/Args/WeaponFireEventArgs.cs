using UnityEngine;

namespace Reactor.API.GTTOD.Events.Args
{
    public class WeaponFireEventArgs : TypeInstanceEventArgs<GameObject>
    {
        public bool IsPrimaryFire { get; internal set; }

        public WeaponFireEventArgs(GameObject instance) : base(instance) { }
    }
}
