using More_Traits.Extensions;

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
}
