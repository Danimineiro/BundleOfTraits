using RimWorld;
using Verse;

namespace More_Traits.ThoughtWorkers
{
    public class ThoughtWorker_Moody : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            switch (p.needs.mood.CurLevelPercentage)
            {
                case float n when n >= 0.8f:
                    return ThoughtState.ActiveAtStage(3);

                case float n when n >= 0.6f && n < 0.8f:
                    return ThoughtState.ActiveAtStage(2);

                //Intentionally no thought between 0.4 and 0.6
                case float n when n >= 0.2f && n < 0.4f:
                    return ThoughtState.ActiveAtStage(1);

                case float n when n < 0.2f:
                    return ThoughtState.ActiveAtStage(0);

                default:
                    return ThoughtState.Inactive;
            }
        }
    }
}
