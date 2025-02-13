namespace More_Traits.ThoughtWorkers;

public class ThoughtWorker_PyrophobiaOnFire : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p) => p.IsBurning();
}
