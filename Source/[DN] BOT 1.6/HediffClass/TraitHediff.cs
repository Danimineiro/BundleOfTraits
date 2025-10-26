using More_Traits.Extensions;
using More_Traits.ModExtensions;

namespace More_Traits.HediffClass;
public abstract class TraitHediff : Hediff
{
    private const int checkInterval = 300;

    protected TraitDef? traitDef;

    public override bool ShouldRemove => traitDef is null || !pawn.HasTrait(traitDef);

    public override void TickInterval(int delta)
    {
        if (!pawn.IsHashIntervalTick(checkInterval, delta)) return;
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
