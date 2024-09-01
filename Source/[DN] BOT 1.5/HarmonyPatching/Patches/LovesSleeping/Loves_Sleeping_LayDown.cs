using RimWorld;
using Verse.AI;
using Verse;
using More_Traits.Extensions;
using More_Traits.DefOfs;

namespace More_Traits.HarmonyPatching.Patches.LovesSleeping;

internal static class Loves_Sleeping_LayDown
{
    private const float ExtraMult = 0.000015f;

    internal static void AddLoves_SleepingActions(Toil toil, JobDriver_LayDown driver)
    {
        if (!driver.CanRest) return;
        if (!driver.CanSleep) return;
        if (driver.Bed == null) return;
        if (!driver.pawn.HasTrait(BOT_TraitDefOf.BOT_Loves_Sleeping)) return;

        void tickAction()
        {
            Pawn actor = toil.actor;
            JobDriver curDriver = actor.jobs.curDriver;

            if (!curDriver.asleep) return;
            if (actor.CurJob.GetTarget(TargetIndex.A).Thing is not Building_Bed bed) return;

            //We're somehow at ~50% rest gain
            float recGain = bed.GetStatValue(StatDefOf.BedRestEffectiveness) * actor.GetStatValue(StatDefOf.RestRateMultiplier) * ExtraMult;
            actor.needs.joy?.GainJoy(recGain, BOT_JoyKindDefOf.BOT_LovesSleepSleeping);
        }

        void finishAction()
        {
            Pawn actor = toil.actor;

            if (actor.needs.rest.CurLevelPercentage <= 0.9) return;
            actor.TryGainMemory(BOT_ThoughtDefOf.BOT_LovesSleepWellRested, 0);
        }

        toil.tickAction += tickAction;
        toil.AddFinishAction(finishAction);
    }
}
