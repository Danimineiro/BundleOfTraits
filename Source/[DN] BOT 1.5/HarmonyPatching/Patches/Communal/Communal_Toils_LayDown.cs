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
using Verse.AI;

namespace More_Traits.HarmonyPatching.Patches.Communal
{
    public static class Communal_Toils_LayDown
    {
        public static void AddCommunalActions(Toil toil, JobDriver_LayDown driver)
        {
            if (!driver.CanRest) return;
            if (!driver.CanSleep) return;
            if (driver.Bed == null) return;
            if (!driver.pawn.HasTrait(BOT_TraitDefOf.BOT_Communal)) return;

            void communalFinishAction()
            {
                Pawn actor = toil.actor;
                ApplyBedThoughts_Prefix(actor);
            }

            toil.AddFinishAction(communalFinishAction);
        }

        private static void ApplyBedThoughts_Prefix(Pawn actor)
        {
            if (actor.needs.mood == null) return;

            Building_Bed building_Bed = actor.CurrentBed();

            actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(BOT_ThoughtDefOf.BOT_Communal_SleptInBarracks);
            actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(BOT_ThoughtDefOf.BOT_Communal_SleptInBedroom);

            if (building_Bed != null) return;
            if (building_Bed.ForPrisoners) return;
            if (building_Bed != actor.ownership.OwnedBed) return;
            if (building_Bed.CostListAdjusted().Count == 0) return;

            if (actor.HasTrait(TraitDefOf.Ascetic)) return;
            if (actor.GetRoom(RegionType.Set_All).PsychologicallyOutdoors) return;

            ThoughtDef thoughtDef = null;

            Room bedRoom = building_Bed.GetRoom(RegionType.Set_All);
            if (bedRoom.Role == RoomRoleDefOf.Bedroom)
            {
                thoughtDef = BOT_ThoughtDefOf.BOT_Communal_SleptInBedroom;
            }
            else if (building_Bed.GetRoom(RegionType.Set_All).Role == RoomRoleDefOf.Barracks)
            {
                thoughtDef = BOT_ThoughtDefOf.BOT_Communal_SleptInBarracks;
            }

            if (thoughtDef == null) return;

            int scoreStageIndex = RoomStatDefOf.Impressiveness.GetScoreStageIndex(bedRoom.GetStat(RoomStatDefOf.Impressiveness));
            if (thoughtDef.stages[scoreStageIndex] == null) return;

            int owners = -1; //The pawn themselves

            foreach (Building_Bed bed in bedRoom.ContainedBeds)
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
