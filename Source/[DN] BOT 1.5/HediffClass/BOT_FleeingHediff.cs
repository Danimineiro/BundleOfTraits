using More_Traits.Extensions;
using More_Traits.ModExtensions;
using More_Traits.ThinkNodes;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace More_Traits.HediffClass
{
    public class BOT_FleeingHediff : Hediff
    {
        private readonly BOT_JobGiverFleeing jobGiver = new BOT_JobGiverFleeing();
        private TraitDef traitDef;

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

        public override bool ShouldRemove
        {
            get
            {
                if (Find.TickManager.TicksGame % 300 != 0) return false;
                return traitDef is null || !pawn.HasTrait(traitDef);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref traitDef, nameof(traitDef));
        }
    }
}
