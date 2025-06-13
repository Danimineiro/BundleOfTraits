using More_Traits.Extensions;
using More_Traits.ModExtensions;
using Verse.AI;

namespace More_Traits.MentalStates.MentalStateWorkers;

public class BOT_TraitMentalStateWorker : MentalStateWorker
{
    private TraitDef? traitDef;
    private TraitDef TraitDef => traitDef ??= def.GetModExtension<BOT_MentalStateExtension>().traitDef; 

    public override bool StateCanOccur(Pawn pawn)
    {
        if (!pawn.HasTrait(TraitDef)) return false;
        return base.StateCanOccur(pawn);
    }
}
