using More_Traits.DefOfs;
using More_Traits.Extensions;

namespace More_Traits.HarmonyPatching.Patches.IngestionThoughts;
public static class IngestionPatches
{
    public static bool IsThoughtFromIngestionDisallowed(ThoughtDef thought, ThingDef ingestible, ref bool __result, TraitSet __instance)
    {
        if (!__instance.HasTrait(BOT_TraitDefOf.BOT_SoylentNeed)) return true;
        
        if (__instance.IsThoughtDisallowed(thought))
        {
            __result = true;
            return false;
        }

        if (ingestible == ThingDefOf.MealNutrientPaste)
        {
            if (thought == BOT_ThoughtDefOf.BOT_SoylentNeed_AteNutrientPasteMeal)
            {
                __result = false;
                return false;
            }

            __result = true;
            return false;
        }

        if (thought.stages.UnsafeContains(stage => stage.baseMoodEffect > 0))
        {
            BOT_TraitDefOf.BOT_SoylentNeed.degreeDatas[0].disallowedThoughts.Add(thought);

            __result = true;
            return false;
        }

        return true;
    }
}
