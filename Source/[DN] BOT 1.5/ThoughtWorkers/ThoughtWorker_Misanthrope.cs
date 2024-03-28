using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.ThoughtWorkers
{
    public class ThoughtWorker_Misanthrope : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn misanthrope, Pawn otherPawn)
        {
            return misanthrope.RaceProps.Humanlike && otherPawn.RaceProps.Humanlike && RelationsUtility.PawnsKnowEachOther(misanthrope, otherPawn);
        }
    }
}
