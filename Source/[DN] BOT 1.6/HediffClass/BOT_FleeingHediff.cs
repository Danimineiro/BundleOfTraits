using More_Traits.ThinkNodes;
using Verse.AI;

namespace More_Traits.HediffClass;

public class BOT_FleeingHediff : TraitHediff
{
    private readonly BOT_JobGiverFleeing jobGiver = new();

    public override void Tick()
    {
        base.Tick();
        if (!pawn.IsHashIntervalTick(120)) return;

        Job job = jobGiver.TryIssueJobPackage(pawn, new JobIssueParams()).Job;
        if (job is null) return;

        pawn.jobs.StartJob(job, JobCondition.Incompletable);
    }
}
