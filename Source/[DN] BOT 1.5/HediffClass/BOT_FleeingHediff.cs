using More_Traits.ModExtensions;
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

    public override void PostMake()
    {
        base.PostMake();
        traitDef = pawn.story.traits.allTraits.FirstOrDefault(trait => trait.def.GetModExtension<BOT_TraitExtension>()?.hediffDef == def)?.def;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Defs.Look(ref traitDef, nameof(traitDef));
    }
}
