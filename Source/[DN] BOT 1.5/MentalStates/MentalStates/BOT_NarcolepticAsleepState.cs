using More_Traits.DefOfs;
using RimWorld;
using Verse;
using Verse.AI;

namespace More_Traits.MentalStates.MentalStates;

public class BOT_NarcolepticAsleepState : MentalState
{
    public override bool AllowRestingInBed => false;

    public override RandomSocialMode SocialModeMax() => RandomSocialMode.Off;

    public override void PreStart()
    {
        base.PreStart();
        pawn.caller?.DoCall();
        pawn.jobs.SuspendCurrentJob(JobCondition.Ongoing);
        pawn.jobs.StartJob(new Job(JobDefOf.Wait), JobCondition.InterruptForced);
        if (PawnUtility.ShouldSendNotificationAbout(pawn)) Messages.Message("BOTNarcolepticInvoluntarySleep".Translate(pawn.LabelShort, pawn), pawn, MessageTypeDefOf.NegativeEvent);
    }

    public override void MentalStateTick()
    {
        base.MentalStateTick();
        pawn.needs.rest.TickResting(StatDefOf.BedRestEffectiveness.valueIfMissing);

        Thing spawnedParentOrMe;
        if (pawn.IsHashIntervalTick(100) && (spawnedParentOrMe = pawn.SpawnedParentOrMe) != null && !spawnedParentOrMe.Position.Fogged(spawnedParentOrMe.Map))
        {
            FleckDef fleckDef = FleckDefOf.SleepZ;
            float velocitySpeed = 0.42f;
            if (pawn.ageTracker.CurLifeStage.developmentalStage == DevelopmentalStage.Baby || pawn.ageTracker.CurLifeStage.developmentalStage == DevelopmentalStage.Newborn)
            {
                fleckDef = FleckDefOf.SleepZ_Tiny;
                velocitySpeed = 0.25f;
            }
            else if (pawn.ageTracker.CurLifeStage.developmentalStage == DevelopmentalStage.Child)
            {
                fleckDef = FleckDefOf.SleepZ_Small;
                velocitySpeed = 0.33f;
            }
            FleckMaker.ThrowMetaIcon(spawnedParentOrMe.Position, spawnedParentOrMe.Map, fleckDef, velocitySpeed);
        }
    }

    public override void PostEnd()
    {
        base.PostEnd();
        if (pawn.CurJobDef == JobDefOf.Wait) pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
        pawn.needs.mood.thoughts.memories.TryGainMemory(BOT_ThoughtDefOf.BOT_Narcoleptic_Awake);
    }
}
