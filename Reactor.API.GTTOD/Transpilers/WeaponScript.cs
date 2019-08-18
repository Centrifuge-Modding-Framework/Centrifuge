using Harmony;

namespace Reactor.API.GTTOD.Transpilers
{
    internal static partial class WeaponScript
    {
        public static void ApplyAll(HarmonyInstance harmony)
        {
            PrimaryFire.Apply(harmony);
            SecondaryFire.Apply(harmony);
        }
    }
}
