using Reactor.API.GTTOD.Events.Args;
using System;

namespace Reactor.API.GTTOD.Events
{
    public static class EnemyNPC
    {
        public static event EventHandler<EnemyDeathEventArgs> InfantryDied;
        public static event EventHandler<EnemyDeathEventArgs> DroneDied;
        public static event EventHandler<EnemyDeathEventArgs> TurretCrabDied;

        internal static void InvokeInfantryDied(Infantry infantry)
        {
            InfantryDied?.Invoke(null, new EnemyDeathEventArgs(infantry.gameObject));
        }

        internal static void InvokeDroneDied(Drone drone)
        {
            DroneDied?.Invoke(null, new EnemyDeathEventArgs(drone.gameObject));
        }

        internal static void InvokeTurretCrabDied(TurretCrab turretCrab)
        {
            TurretCrabDied?.Invoke(null, new EnemyDeathEventArgs(turretCrab.gameObject));
        }
    }
}
