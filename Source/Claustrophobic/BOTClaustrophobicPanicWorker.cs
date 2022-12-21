using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace More_Traits
{
	public class BOTClaustrophobicPanicWorker : MentalBreakWorker
	{
        public override bool BreakCanOccur(Pawn pawn)
        {
            if (pawn.Awake() && pawn.GetRoom() is Room room && room != null)
            {
                RoomStatDef spaceDef = RoomStatDefOf.Space;

                if (spaceDef.GetScoreStageIndex(room.GetStat(spaceDef)) > 1) return false;
            }

            return pawn.Spawned && base.BreakCanOccur(pawn);
        }

        public override bool TryStart(Pawn pawn, string reason, bool causedByMood)
        {
            pawn.health.AddHediff(BOTHediffDefOf.BOT_ClaustrophobicBreakdown);
            return base.TryStart(pawn, reason, causedByMood);
        }
    }
}
