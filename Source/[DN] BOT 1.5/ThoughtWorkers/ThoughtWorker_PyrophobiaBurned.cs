using More_Traits.DefOfs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.ThoughtWorkers
{
    public class ThoughtWorker_PyrophobiaBurned : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (p.health.hediffSet.GetFirstHediffOfDef(BOT_HediffDefOf.Burn) != null)
            {
                if (ModsConfig.IdeologyActive && p.Ideo.HasPrecept(BOT_PreceptDefOf.Pain_Idealized))
                {
                    return ThoughtState.ActiveAtStage(1);
                }
                return ThoughtState.ActiveAtStage(0);
            }
            return false;
        }
    }
}
