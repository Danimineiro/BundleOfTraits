using More_Traits.DefOfs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using More_Traits.Extensions;

namespace More_Traits.HarmonyPatching.Patches.SleepyHead
{
    internal static class SleepyHead_LayDown
    {
        internal static Toil SleepyHeadToil(JobDriver_LayDown jobDriver)
        {
            if (!jobDriver.CanRest) return null;
            if (!jobDriver.CanSleep) return null;
            if (jobDriver.Bed == null) return null;
            if (!jobDriver.pawn.HasTrait(BOT_TraitDefOf.BOT_Sleepyhead)) return null;

            Toil toil = ToilMaker.MakeToil(nameof(SleepyHeadToil));
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.FailOnBedNoLongerUsable(TargetIndex.A);
            toil.defaultDuration = 5000;

            void initAction()
            {
                Pawn actor = toil.actor;
                Job curJob = actor.CurJob;
                JobDriver_LayDown driver = actor.jobs.curDriver as JobDriver_LayDown;
                Building_Bed bed = actor.CurJob.GetTarget(TargetIndex.A).Thing as Building_Bed;

                if (!bed.OccupiedRect().Contains(actor.Position))
                {
                    Log.Error($"Can't start {nameof(SleepyHeadToil)} because pawn is not in the bed. pawn: {actor}");
                    actor.jobs.EndCurrentJob(JobCondition.Errored);
                    return;
                }

                driver.asleep = true;
                curJob.forceSleep = true;
                actor.jobs.posture = PawnPosture.LayingInBed;
                PortraitsCache.SetDirty(actor);
            }

            void tickAction()
            {
                Pawn actor = toil.actor;
                JobDriver_LayDown driver = actor.jobs.curDriver as JobDriver_LayDown;
                Building_Bed bed = actor.CurJob.GetTarget(TargetIndex.A).Thing as Building_Bed;

                Type boolType = typeof(bool);
                MethodInfo ApplyBedEffects = typeof(Toils_LayDown).GetMethod("ApplyBedRelatedEffects", BindingFlags.Static | BindingFlags.NonPublic);
                ApplyBedEffects.Invoke(null, new object[] { actor, bed, true, true, false });
            }

            void finishAction()
            {
                toil.actor.TryGainMemory(BOT_ThoughtDefOf.BOT_SleepyHeadContinuesSleeping, 0);
            }

            toil.initAction = initAction;
            toil.tickAction = tickAction;
            toil.AddFinishAction(finishAction);
            return toil;
        }

        public static void LookForOtherJobs_Postfix(JobDriver_LayDown __instance, ref bool __result)
        {
            if (!(__instance is JobDriver_LayDown)) return;
            if (!__instance.pawn?.HasTrait(BOT_TraitDefOf.BOT_Sleepyhead) == true) return;

            __result = false;
        }

        internal static void AddPreTick(Toil toil, JobDriver_LayDown jobDriver)
        {
            if (!jobDriver.CanRest) return;
            if (!jobDriver.CanSleep) return;
            if (jobDriver.Bed == null) return;
            if (!jobDriver.pawn.HasTrait(BOT_TraitDefOf.BOT_Sleepyhead)) return;

            void preTickAction()
            {
                Pawn actor = toil.actor;
                Job curJob = actor.CurJob;
                JobDriver_LayDown driver = actor.jobs.curDriver as JobDriver_LayDown;

                if (!driver.asleep && driver.CanSleep)
                {
                    driver.ReadyForNextToil();
                }
            }

            toil.AddPreTickAction(preTickAction);
        }
    }
}
