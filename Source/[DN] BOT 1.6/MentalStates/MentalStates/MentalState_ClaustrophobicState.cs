using Verse.AI;
using More_Traits.DefOfs;

namespace More_Traits.MentalStates.MentalStates;

public class MentalState_ClaustrophobicState : MentalState
{
    public new bool AllowRestingInBed => false;

    public override RandomSocialMode SocialModeMax() => RandomSocialMode.Off;

    public override void MentalStateTick()
    {
        if (pawn.CurJob?.def != JobDefOf.Wait_MaintainPosture)
        {
            pawn.jobs.StartJob(new Job(JobDefOf.Wait_MaintainPosture), JobCondition.InterruptForced);
        }

        base.MentalStateTick();
    }

    public override void PostEnd()
    {
        if (pawn.health.hediffSet.GetFirstHediffOfDef(BOT_HediffDefOf.BOT_ClaustrophobicBreakdown) is Hediff hediff) pawn.health.RemoveHediff(hediff);
        base.PostEnd();
    }
}
