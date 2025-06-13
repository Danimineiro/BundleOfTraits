using More_Traits.DefOfs;
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

    public override void MentalStateTick(int delta)
    {
        base.MentalStateTick(delta);
        pawn.needs.rest.TickResting(StatDefOf.BedRestEffectiveness.valueIfMissing);

        if (!pawn.IsHashIntervalTick(100, delta)) return;
        if (pawn.SpawnedParentOrMe is not Thing spawnedParentOrMe) return;
        if (spawnedParentOrMe.Position.Fogged(spawnedParentOrMe.Map)) return;
        
        (FleckDef fleckDef, float velocitySpeed) = pawn.ageTracker.CurLifeStage.developmentalStage switch
        {
            DevelopmentalStage.Baby or DevelopmentalStage.Newborn => (FleckDefOf.SleepZ_Tiny, .25f),
            DevelopmentalStage.Child => (FleckDefOf.SleepZ_Small, .33f),
            _ => (FleckDefOf.SleepZ, .42f)
        };

        FleckMaker.ThrowMetaIcon(spawnedParentOrMe.Position, spawnedParentOrMe.Map, fleckDef, velocitySpeed);
    }

    public override void PostEnd()
    {
        base.PostEnd();
        if (pawn.CurJobDef == JobDefOf.Wait) pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
        pawn.needs.mood.thoughts.memories.TryGainMemory(BOT_ThoughtDefOf.BOT_Narcoleptic_Awake);
    }
}
