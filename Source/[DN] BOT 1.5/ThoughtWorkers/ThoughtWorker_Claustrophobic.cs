using RimWorld;
using Verse;

namespace More_Traits.ThoughtWorkers
{
    public class ThoughtWorker_Claustrophobic : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn pawn)
        {
            if (!pawn.Awake()) return ThoughtState.Inactive;
            if (pawn.GetRoom() is not Room room) return ThoughtState.Inactive;

            RoomStatDef spaceDef = RoomStatDefOf.Space;
            int score = spaceDef.GetScoreStageIndex(room.GetStat(RoomStatDefOf.Space));

            if (score > 1) return ThoughtState.Inactive;

            return ThoughtState.ActiveAtStage(score);
        }
    }
}
