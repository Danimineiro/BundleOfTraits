namespace More_Traits.ThoughtWorkers;

public class ThoughtWorker_PacifistIsCarryingWeapon : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        return p.equipment.Primary != null;
    }
}
