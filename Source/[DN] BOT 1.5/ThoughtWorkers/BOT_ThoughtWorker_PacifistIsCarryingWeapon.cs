using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.ThoughtWorkers
{
    public class BOT_ThoughtWorker_PacifistIsCarryingWeapon : ThoughtWorker
    {

        public class BOT_ThoughtWorker_IsCarryingWeapon : ThoughtWorker
        {
            protected override ThoughtState CurrentStateInternal(Pawn p)
            {
                return p.equipment.Primary != null;
            }
        }
    }
}
