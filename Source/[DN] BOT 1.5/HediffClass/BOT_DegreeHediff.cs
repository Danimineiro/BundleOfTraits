using More_Traits.ModExtensions;
using UnityEngine;

namespace More_Traits.HediffClass;

public class BOT_DegreeHediff : TraitHediff
{
    private BOT_HediffExtension? extension;

    private int degreeCache = 0;

    public BOT_HediffExtension Extension => extension ??= def.GetModExtension<BOT_HediffExtension>();

    public override Color LabelColor => Extension.stageColors?[degreeCache] ?? ColorLibrary.GrassGreen;

    public override void PostMake()
    {
        base.PostMake();
        degreeCache = pawn.story.traits.GetTrait(traitDef = Extension.traitDef).Degree;
    }

    public override int CurStageIndex => degreeCache;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Defs.Look(ref traitDef, nameof(traitDef));
        Scribe_Values.Look(ref degreeCache, nameof(degreeCache));
    }
}
