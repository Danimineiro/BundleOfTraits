using More_Traits.ModExtensions;
using UnityEngine;

namespace More_Traits.HediffClass;

public class BOT_DegreeHediff : TraitHediff
{
    private int degreeCache = 0;
    private BOT_HediffExtension? extension;

    public BOT_HediffExtension Extension => extension ??= def.GetModExtension<BOT_HediffExtension>();

    public override Color LabelColor => Extension.stageColors?[degreeCache] ?? ColorLibrary.GrassGreen;

    public override void PostMake()
    {
        degreeCache = pawn.story.traits.GetTrait(traitDef = Extension.traitDef).Degree;
    }

    public override int CurStageIndex => degreeCache;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref degreeCache, nameof(degreeCache));
    }
}
