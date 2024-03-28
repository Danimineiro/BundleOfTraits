using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using More_Traits.DefOfs;
using More_Traits.Extensions;

namespace More_Traits.HarmonyPatching.Patches
{
    public class EclecticPalate_FinalizeIngestPatch
    {
        public static void EclecticPalate_Postfix(Pawn ingester, TargetIndex ingestibleInd, ref Toil __result)
        {
            if (!(ingester.CurJob.GetTarget(ingestibleInd).Thing is Thing food)) return;
            if (!food.def.ingestible.IsMeal) return;

            void action()
            {
                List<ThingDef> ingredients = food.TryGetComp<CompIngredients>().ingredients;
                int nrOfIngredients = Math.Min(BOT_ThoughtDefOf.BOT_EclecticPalateAte.stages.Count - 1, Math.Max(0, ingredients.Count - 1));

                ingester.TryGainMemory(BOT_ThoughtDefOf.BOT_EclecticPalateAte, nrOfIngredients);
            }

            __result.AddFinishAction(action);
        }
    }
}
