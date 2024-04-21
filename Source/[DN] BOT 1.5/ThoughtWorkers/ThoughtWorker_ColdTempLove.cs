using More_Traits.DefOfs;
using More_Traits.ThoughtWorkers.Enums;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.ThoughtWorkers
{
    public class ThoughtWorker_ColdTempLove : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn pawn)
        {
            //if (IsHotLover(pawn)) return ThoughtState.Inactive;

            float comfTempDiff = pawn.GetStatValue(StatDefOf.ComfyTemperatureMin, true) - pawn.AmbientTemperature;
            if (pawn.AmbientTemperature < 5f && comfTempDiff < 10f && !IsHotLover(pawn)) return ThoughtState.ActiveAtStage(4);
            if (comfTempDiff <= 0f) return ThoughtState.Inactive; //Temperatur is lower than comfy levels

            int thoughtStage = Math.Min(3, ((int)comfTempDiff) / 10);
            if (ModsConfig.IdeologyActive && pawn.Ideo.HasPrecept(PreceptDefOf.Temperature_Tough))
            {
                thoughtStage -= 2;
            }
            if (thoughtStage >= 0)
            {
                return ThoughtState.ActiveAtStage(thoughtStage);
            }
            return ThoughtState.Inactive;
        }

        private static bool IsHotLover(Pawn pawn)
        {
            return pawn.story.traits.DegreeOfTrait(BOT_TraitDefOf.BOT_Temperature_Love) != ((int)TemperaturLove.Cold);
        }
    }
}
