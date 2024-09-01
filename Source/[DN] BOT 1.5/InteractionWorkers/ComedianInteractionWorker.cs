using More_Traits.DefOfs;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace More_Traits.InteractionWorkers;

public class ComedianInteractionWorker : InteractionWorker
{
    private const float BaseSelectionWeight = 0.1f;

    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
        if (!initiator.story.traits.HasTrait(BOT_TraitDefOf.BOT_Comedian)) return 0f;

        return BaseSelectionWeight * compatibilityFactorCurve.Evaluate(initiator.relations.CompatibilityWith(recipient));
    }

    private readonly SimpleCurve compatibilityFactorCurve = new()
    {
        {
            new CurvePoint(-1.5f, 0f)
        },
        {
            new CurvePoint(-0.5f, 0.1f)
        },
        {
            new CurvePoint(0.5f, 1f)
        },
        {
            new CurvePoint(1f, 1.8f)
        },
        {
            new CurvePoint(2f, 3f)
        }
    };

    public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
    {
        if (Rand.Value < 0.3333f)
        {
            //Bad Joke
            Pawn_InteractionsTracker.AddInteractionThought(recipient, initiator, BOT_ThoughtDefOf.BOT_ComedianBadJokeMood);
            Pawn_InteractionsTracker.AddInteractionThought(recipient, initiator, BOT_ThoughtDefOf.BOT_ComedianBadJokeOpinion);
            Pawn_InteractionsTracker.AddInteractionThought(initiator, recipient, BOT_ThoughtDefOf.BOT_ComedianBadJokeOpinionSelf);
        }
        else
        {
            //good joke
            Pawn_InteractionsTracker.AddInteractionThought(recipient, initiator, BOT_ThoughtDefOf.BOT_ComedianGoodJokeMood);
            Pawn_InteractionsTracker.AddInteractionThought(recipient, initiator, BOT_ThoughtDefOf.BOT_ComedianGoodJokeOpinion);
            Pawn_InteractionsTracker.AddInteractionThought(initiator, recipient, BOT_ThoughtDefOf.BOT_ComedianGoodJokeOpinionSelf);
        }

        base.Interacted(initiator, recipient, extraSentencePacks, out letterText, out letterLabel, out letterDef, out lookTargets);
    }
}
