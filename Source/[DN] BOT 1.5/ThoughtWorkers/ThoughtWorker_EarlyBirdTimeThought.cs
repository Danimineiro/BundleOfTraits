using RimWorld;
using Verse;

namespace More_Traits.ThoughtWorkers;

public class ThoughtWorker_EarlyBirdTimeThought : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (p.Awake() && GenLocalDate.HourInteger(p) >= 6 && GenLocalDate.HourInteger(p) < 13)
        {
            return ThoughtState.ActiveAtStage(0);
        }
        if (p.Awake() && GenLocalDate.HourInteger(p) >= 17)
        {
            return ThoughtState.ActiveAtStage(1);
        }

        return false;
    }
}
