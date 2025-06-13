using More_Traits.DefOfs;
using More_Traits.Extensions;
using UnityEngine;

namespace More_Traits.HarmonyPatching.Patches.Chinophile;

public static class Chinophile_Pawn
{
    public static void TicksPerMove(ref float __result, Pawn __instance, bool diagonal)
    {
        if (!__instance.Spawned) return;
        if (__instance.debugMaxMoveSpeed) return;
        if (!__instance.HasTrait(BOT_TraitDefOf.BOT_Chionophile)) return;

        __result /= Mathf.Lerp(1f, 3.5f, __instance.Map.snowGrid.GetDepth(__instance.Position));
        __result = Mathf.Clamp(__result, 1f, 450f);
    }
}
