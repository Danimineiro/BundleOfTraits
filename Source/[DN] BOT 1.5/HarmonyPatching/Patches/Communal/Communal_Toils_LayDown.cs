using More_Traits.DefOfs;
using More_Traits.Extensions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace More_Traits.HarmonyPatching.Patches.Communal
{
    public static class Communal_Toils_LayDown
    {
        public static void Postfix(Pawn actor)
        {
            if (actor.needs.mood == null || !actor.HasTrait(BOT_TraitDefOf.BOT_Communal)) return;
            Building_Bed building_Bed = actor.CurrentBed();

            actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(BOT_ThoughtDefOf.BOT_Communal_SleptInBarracks);
            actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(BOT_ThoughtDefOf.BOT_Communal_SleptInBedroom);

            if (actor.GetRoom(RegionType.Set_All).PsychologicallyOutdoors || building_Bed == null || building_Bed.CostListAdjusted().Count == 0) return;

            if (building_Bed != null && building_Bed == actor.ownership.OwnedBed && !building_Bed.ForPrisoners && !actor.HasTrait(TraitDefOf.Ascetic))
            {
                ThoughtDef thoughtDef = null;
                if (building_Bed.GetRoom(RegionType.Set_All).Role == RoomRoleDefOf.Bedroom)
                {
                    thoughtDef = BOT_ThoughtDefOf.BOT_Communal_SleptInBedroom;
                }
                else if (building_Bed.GetRoom(RegionType.Set_All).Role == RoomRoleDefOf.Barracks)
                {
                    thoughtDef = BOT_ThoughtDefOf.BOT_Communal_SleptInBarracks;
                }
                if (thoughtDef != null)
                {
                    int scoreStageIndex = RoomStatDefOf.Impressiveness.GetScoreStageIndex(building_Bed.GetRoom(RegionType.Set_All).GetStat(RoomStatDefOf.Impressiveness));
                    if (thoughtDef.stages[scoreStageIndex] != null)
                    {
                        int owners = -1; //The pawn themselves

                        foreach (Building_Bed bed in building_Bed.GetRoom(RegionType.Set_All).ContainedBeds)
                        {
                            owners += bed.OwnersForReading.Count;
                        }

                        int nrOfOthers = Mathf.Clamp(owners, 0, 20);
                        int thoughtStage = nrOfOthers > 0 ? nrOfOthers / 2 + 1 : 0;

                        actor.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(BOT_ThoughtDefOf.BOT_Communal_Sharing, thoughtStage));
                        actor.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(thoughtDef, scoreStageIndex));
                    }
                }
            }
        }
    }
}
