using More_Traits.ThinkNodes;
using Verse.AI;

namespace More_Traits.HediffClass;

public class BOT_FleeingHediff : TraitHediff
{
    private readonly BOT_JobGiverFleeing jobGiver = new();

    public override void TickInterval(int delta)
    {
        base.TickInterval(delta);
        if (!pawn.IsHashIntervalTick(120, delta)) return;

        if (jobGiver.TryIssueJobPackage(pawn, new JobIssueParams()).Job is not Job job) return;

        pawn.jobs.StartJob(job, JobCondition.Incompletable);
    }
}
