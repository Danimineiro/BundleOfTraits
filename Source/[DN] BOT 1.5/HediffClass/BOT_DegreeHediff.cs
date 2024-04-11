using More_Traits.DefOfs;
using More_Traits.Extensions;
using More_Traits.ModExtensions;
using RimWorld;
using Verse;

namespace More_Traits.HediffClass
{
    public class BOT_DegreeHediff : Hediff
    {
        private int degreeCache = 0;
        private TraitDef traitDef;

        public override void PostMake()
        {
            base.PostMake();
            degreeCache = pawn.story.traits.GetTrait(traitDef = def.GetModExtension<BOT_HediffExtension>().traitDef).Degree;
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
    }
}
