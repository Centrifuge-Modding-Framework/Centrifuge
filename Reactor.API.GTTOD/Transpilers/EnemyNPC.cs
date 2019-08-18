using Harmony;

namespace Reactor.API.GTTOD.Transpilers
{
    internal static partial class EnemyNPC
    {
        public static void ApplyAll(HarmonyInstance harmony)
        {
            DroneDie.Apply(harmony);
            InfantryDie.Apply(harmony);
            TurretCrabDie.Apply(harmony);
        }
    }
}
