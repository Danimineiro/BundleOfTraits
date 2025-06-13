using More_Traits.DefOfs;
using More_Traits.Extensions;

namespace More_Traits.HarmonyPatching.Patches;
internal static class CatharsisPatches
{
    public static void MoodOffsetPatch(ref float __result, Thought __instance)
    {
        if (__instance.def != ThoughtDefOf.Catharsis) return;
        if (!__instance.pawn.HasTrait(BOT_TraitDefOf.BOT_PressureCooker)) return;

        __result *= 2f;
    }

    public static void GetDurationTicks(ref int __result, Thought __instance)
    {
        if (__instance.def != ThoughtDefOf.Catharsis) return;
        if (!__instance.pawn.HasTrait(BOT_TraitDefOf.BOT_PressureCooker)) return;

        __result *= 2;
    }
}
