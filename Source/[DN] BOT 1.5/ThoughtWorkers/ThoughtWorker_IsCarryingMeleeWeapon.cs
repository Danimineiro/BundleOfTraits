using RimWorld;
using Verse;

namespace More_Traits.ThoughtWorkers;

public class ThoughtWorker_IsCarryingMeleeWeapon : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn pawn)
    {
        if (pawn.equipment.Primary == null) return false;

        if (!pawn.equipment.Primary.def.IsRangedWeapon) return ThoughtState.ActiveAtStage(0);

        if (!pawn.equipment.Primary.def.techLevel.IsNeolithicOrWorse()) return ThoughtState.ActiveAtStage(1);

        return false;
    }
}
