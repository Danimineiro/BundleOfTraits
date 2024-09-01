using RimWorld;
using System;
using Verse.AI;
using Verse;
using More_Traits.Extensions;
using More_Traits.DefOfs;

namespace More_Traits.HarmonyPatching.Patches.Eclectic;

public class EclecticPalate_FinalizeIngestPatch
{
    public static void Postfix(Pawn ingester, TargetIndex ingestibleInd, ref Toil __result)
    {
        if (ingester.CurJob.GetTarget(ingestibleInd).Thing is not Thing food) return;
        if (!(food.def.IsNutritionGivingIngestible && food.def.ingestible.IsMeal)) return;
        if (!ingester.HasTrait(BOT_TraitDefOf.BOT_Eclectic_Palate)) return;

        void action()
        {
            int ingredientsCount = food.TryGetComp<CompIngredients>()?.ingredients?.Count ?? 0;
            int nrOfIngredients = Math.Min(BOT_ThoughtDefOf.BOT_EclecticPalateAte.stages.Count - 1, Math.Max(0, ingredientsCount - 1));

            ingester.TryGainMemory(BOT_ThoughtDefOf.BOT_EclecticPalateAte, nrOfIngredients);
        }

        __result.AddFinishAction(action);
    }
}
