using More_Traits.DefOfs;
using More_Traits.Extensions;
using More_Traits.ModExtensions;
using RimWorld;
using UnityEngine;
using Verse;

namespace More_Traits.HediffClass
{
    public class BOT_DegreeHediff : Hediff
    {
        private BOT_HediffExtension extension;
        private TraitDef traitDef;

        private int degreeCache = 0;

        public BOT_HediffExtension Extension => extension ?? (extension = def.GetModExtension<BOT_HediffExtension>());

        public override string Label => traitDef.DataAtDegree(degreeCache).label;

        public override string Description => traitDef.DataAtDegree(degreeCache).description.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true).Resolve();

        public override Color LabelColor => Extension.stageColors?[degreeCache] ?? ColorLibrary.GrassGreen;

        public override void PostMake()
        {
            base.PostMake();
            degreeCache = pawn.story.traits.GetTrait(traitDef = Extension.traitDef).Degree;
        }

        public override int CurStageIndex => degreeCache;

        public override bool ShouldRemove
        {
            get
            {
                if (Find.TickManager.TicksGame % 300 != 0) return false;
                return !pawn.HasTrait(traitDef);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref traitDef, nameof(traitDef));
            Scribe_Values.Look(ref degreeCache, nameof(degreeCache));
        }
    }
}
