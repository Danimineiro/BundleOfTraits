using More_Traits.Extensions;
using More_Traits.ModExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
