using UnityEngine;
using Verse.AI;

namespace More_Traits.HediffClass;
public class BOT_Composed : TraitHediff
{
    public override int CurStageIndex { get; } = 0;
    public float buffAmount = 0f;

    public override HediffStage CurStage
    {
        get
        {
            HediffStage stage = base.CurStage;

            if (pawn.MentalStateDef != null) return stage;
            stage.statFactors[0].value = 1 + buffAmount;

            return base.CurStage;
        }
    }

    public override void PostTick()
    {
        MentalBreaker mentalBreaker = pawn.mindState.mentalBreaker;
        buffAmount = Mathf.Lerp(0f, .5f, 1f - Mathf.Clamp01((mentalBreaker.CurMood - mentalBreaker.BreakThresholdExtreme) / (1f - mentalBreaker.BreakThresholdExtreme - mentalBreaker.BreakThresholdMinor)));
    }
}
