using More_Traits.DefOfs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using More_Traits.Extensions;

namespace More_Traits.HarmonyPatching.Patches.LovesSleeping
{
    public class Loves_Sleeping_LayDown
    {
        private const float ExtraMult = 0.00002f;

        public static void Postfix(Toil __result, TargetIndex bedOrRestSpotIndex, bool hasBed, bool canSleep = true, bool gainRestAndHealth = true, PawnPosture noBedLayingPosture = PawnPosture.LayingOnGroundNormal)
        {
            Pawn pawn = __result.actor;
            if (!gainRestAndHealth) return;
            if (!canSleep) return;
            if (!hasBed) return;

            void tickAction()
            {
                JobDriver curDriver = __result.actor.jobs.curDriver;
                if (!__result.actor.HasTrait(BOT_TraitDefOf.BOT_Loves_Sleeping)) return;
                if (!curDriver.asleep) return;
                if (!(__result.actor.CurJob.GetTarget(bedOrRestSpotIndex).Thing is Building_Bed bed)) return;

                //We're somehow at ~50% rest gain
                float recGain = bed.GetStatValue(StatDefOf.BedRestEffectiveness) * __result.actor.GetStatValue(StatDefOf.RestRateMultiplier) * ExtraMult;
                __result.actor.needs.joy.GainJoy(recGain, BOT_JoyKindDefOf.BOT_LovesSleepSleeping);
            }

            void finishAction()
            {
                if (__result.actor.needs.rest.CurLevelPercentage <= 0.9) return;
                __result.actor.TryGainMemory(BOT_ThoughtDefOf.BOT_LovesSleepWellRested, 0);
            }

            __result.tickAction += tickAction;
            __result.AddFinishAction(finishAction);

            ////Deals with sleepyheads not wanting to wake up
            //if ((___pawn.CurJobDef == JobDefOf.LayDown || ___pawn.CurJobDef == JobDefOf.LayDownAwake) && newJob.def != JobDefOf.LayDown && ___pawn.story.traits.HasTrait(BOTTraitDefOf.BOT_Sleepyhead) && ___pawn.needs.rest.CurCategory == RestCategory.Rested && !Manager.GetSleepyHeadSet().Contains(___pawn))
            //{
            //    if (Rand.Value > 0.3)
            //    {
            //        newJob = ___pawn.CurJob;
            //        newJob.def = JobDefOf.LayDown;
            //        newJob.forceSleep = true;
            //        ___pawn.needs.rest.CurLevelPercentage = 0.30f;
            //        ___pawn.TryGainMemory(BOTThoughtDefOf.BOT_SleepyHeadContinuesSleeping, 0);
            //        Manager.GetSleepyHeadSet().Add(___pawn);
            //    }
            //}
        }
    }
}
