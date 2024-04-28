using More_Traits.DefOfs;
using More_Traits.Extensions;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace More_Traits.ThoughtWorkers
{
    public class ThoughtWorker_GregariousCompany : ThoughtWorker
    {
        private static readonly Dictionary<Pawn, int> cachedCountDic = new Dictionary<Pawn, int>();

        public override string PostProcessDescription(Pawn pawn, string description)
        {
            return base.PostProcessDescription(pawn, description.Formatted(GetCountFor(pawn).Named("NUMBER")));
        }

        protected override ThoughtState CurrentStateInternal(Pawn pawn)
        {
            int company = GetCountFor(pawn);

            int nrOfOthers = Mathf.Clamp(company, 0, 20);
            int thoughtStage = nrOfOthers > 0 ? nrOfOthers / 2 + 1 : 0;

            return ThoughtState.ActiveAtStage(thoughtStage);
        }

        private static int GetCountFor(Pawn pawn)
        {
            if (!pawn.IsHashIntervalTick(3000) && cachedCountDic.ContainsKey(pawn)) return cachedCountDic[pawn];
            if (!cachedCountDic.ContainsKey(pawn)) cachedCountDic[pawn] = 0;

            if (pawn.Map != null && !pawn.IsPrisoner)
            {
                cachedCountDic[pawn] = pawn.Map.mapPawns.ColonistCount;
            }
            else if (pawn.IsCaravanMember())
            {
                cachedCountDic[pawn] = pawn.GetCaravan().PawnsListForReading.Sum(member => member.Faction == Faction.OfPlayer ? 1 : 0);
            }

            return cachedCountDic[pawn];
        }
    }
}
