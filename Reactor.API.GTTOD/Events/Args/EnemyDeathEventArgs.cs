using System;
using UnityEngine;

namespace Reactor.API.GTTOD.Events.Args
{
    public class EnemyDeathEventArgs : EventArgs
    {
        public GameObject Enemy { get; }

        public EnemyDeathEventArgs(GameObject enemy)
        {
            Enemy = enemy;
        }
    }
}
