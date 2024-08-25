using More_Traits.Extensions;
using More_Traits.ModExtensions;
using Verse;
using Verse.AI;

namespace More_Traits.MentalStates.MentalStateWorkers
{
    public class BOT_TraitMentalStateWorker : MentalStateWorker
    {
        public override bool StateCanOccur(Pawn pawn)
        {
            if (!pawn.HasTrait(def.GetModExtension<BOT_MentalStateExtension>().traitDef)) return false;
            return base.StateCanOccur(pawn);
        }
    }
}
