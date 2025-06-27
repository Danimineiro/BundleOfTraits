using More_Traits.DefOfs;
using More_Traits.ThoughtWorkers.Enums;
using RimWorld;
using System;
using Verse;

namespace More_Traits.ThoughtWorkers
{
    public class ThoughtWorker_HotTempLove : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn pawn)
        {
            //if (IsColdLover(pawn)) return ThoughtState.Inactive;

            float comfTempDiff = pawn.AmbientTemperature - pawn.GetStatValue(StatDefOf.ComfyTemperatureMax, true);
            if (pawn.AmbientTemperature > 25f && comfTempDiff < 10f && !IsColdLover(pawn)) return ThoughtState.ActiveAtStage(4);
            if (comfTempDiff <= 0f) return ThoughtState.Inactive; //Temperatur is higher than comfy levels

            int thoughtStage = Math.Min(3, (int)comfTempDiff / 10);
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

        private static bool IsColdLover(Pawn pawn)
        {
            return pawn.story.traits.DegreeOfTrait(BOT_TraitDefOf.BOT_Temperature_Love) != (int)TemperaturLove.Heat;
        }
    }
}
