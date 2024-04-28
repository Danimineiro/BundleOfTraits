using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.ThoughtWorkers
{
    public class ThoughtWorker_PyrophobiaOnFire : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p) => p.IsBurning();
    }
}
