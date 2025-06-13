using More_Traits.Extensions;

namespace More_Traits.HarmonyPatching.Patches;

public class HediffTraitPatches
{
    public static void GainTrait(Pawn ___pawn)
    {
        ___pawn.AddTraitHediffs();
    }

    public static void SpawnSetup(Pawn __instance)
    {
        if (__instance.CanHandlePawn()) __instance.AddTraitHediffs();
    }
}
