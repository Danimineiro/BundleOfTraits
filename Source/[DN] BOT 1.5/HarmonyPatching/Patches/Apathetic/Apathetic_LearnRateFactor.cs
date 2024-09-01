using More_Traits.DefOfs;
using More_Traits.Extensions;
using RimWorld;
using Verse;

namespace More_Traits.HarmonyPatching.Patches.Apathetic;

public static class Apathetic_LearnRateFactor
{
    public static bool LearnRateFactor(bool direct, SkillRecord __instance, ref float __result)
    {
        if (DebugSettings.fastLearning) return true;

        Pawn pawn = __instance.Pawn;
        if (!pawn.HasTrait(BOT_TraitDefOf.BOT_Apathetic)) return true;

        __result = 1f;
        if (direct) return false;

        __result *= pawn.GetStatValue(StatDefOf.GlobalLearningFactor, true, -1);
        if (__instance.def == SkillDefOf.Animals) __result *= pawn.GetStatValue(StatDefOf.AnimalsLearningFactor, true, -1);
        if (__instance.LearningSaturatedToday) __result *= HarmonyPatcher.SaturatedXPMult;

        return false;
    }
}
