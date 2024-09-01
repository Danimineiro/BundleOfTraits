using RimWorld;
using Verse.AI;
using Verse;
using More_Traits.DefOfs;

namespace More_Traits.MentalStates.MentalStateWorkers;

public class MentalStateWorker_ClaustrophobicPanic : MentalBreakWorker
{
    public override bool BreakCanOccur(Pawn pawn)
    {
        if (pawn.Awake() && pawn.GetRoom() is Room room && room != null)
        {
            RoomStatDef spaceDef = RoomStatDefOf.Space;

            if (spaceDef.GetScoreStageIndex(room.GetStat(spaceDef)) > 1) return false;
        }

        return pawn.Spawned && base.BreakCanOccur(pawn);
    }

    public override bool TryStart(Pawn pawn, string reason, bool causedByMood)
    {
        pawn.health.AddHediff(BOT_HediffDefOf.BOT_ClaustrophobicBreakdown);
        return base.TryStart(pawn, reason, causedByMood);
    }
}
