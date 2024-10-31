using More_Traits.Extensions;
using More_Traits.ModExtensions;

namespace More_Traits.HediffClass;
public abstract class TraitHediff : Hediff
{
    private const int checkInterval = 300;
    private int nextCheck = 0;

    protected TraitDef? traitDef;

    public override bool ShouldRemove
    {
        get
        {
            int ticksGame = Find.TickManager.TicksGame;
            if (ticksGame < nextCheck) return false;

            nextCheck = ticksGame + checkInterval + pawn.HashOffsetTicks();
            return traitDef is null || !pawn.HasTrait(traitDef);
        }
    }

    public override void PostMake()
    {
        traitDef ??= pawn.story.traits.allTraits.FirstOrDefault(trait => trait.def.GetModExtension<BOT_TraitExtension>()?.hediffDef == def)?.def;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Defs.Look(ref traitDef, nameof(traitDef));
    }
}
