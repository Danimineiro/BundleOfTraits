using System.Collections.Generic;
using RimWorld;
using Verse;

namespace More_Traits
{
	class BOTJoyInteractionWorker : InteractionWorker 
	{
		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            if (initiator.story.traits.HasTrait(BOTTraitDefOf.BOT_Comedian))
            {
                return BaseSelectionWeight * CompatibilityFactorCurve.Evaluate(initiator.relations.CompatibilityWith(recipient));
            }
            return 0f;
        }
        private const float BaseSelectionWeight = 0.1f;

		// Token: 0x04003180 RID: 12672
		private readonly SimpleCurve CompatibilityFactorCurve = new SimpleCurve
		{
			{
				new CurvePoint(-1.5f, 0f),
				true
			},
			{
				new CurvePoint(-0.5f, 0.1f),
				true
			},
			{
				new CurvePoint(0.5f, 1f),
				true
			},
			{
				new CurvePoint(1f, 1.8f),
				true
			},
			{
				new CurvePoint(2f, 3f),
				true
			}
		};

        public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
        {
			if (Rand.Value < 0.3333f)
            {
				//Bad Joke
				Pawn_InteractionsTracker.AddInteractionThought(recipient, initiator, BOTThoughtDefOf.BOT_ComedianBadJokeMood);
				Pawn_InteractionsTracker.AddInteractionThought(recipient, initiator, BOTThoughtDefOf.BOT_ComedianBadJokeOpinion);
				Pawn_InteractionsTracker.AddInteractionThought(initiator, recipient, BOTThoughtDefOf.BOT_ComedianBadJokeOpinionSelf);
			}
            else
            {
				//good joke
				Pawn_InteractionsTracker.AddInteractionThought(recipient, initiator, BOTThoughtDefOf.BOT_ComedianGoodJokeMood);
				Pawn_InteractionsTracker.AddInteractionThought(recipient, initiator, BOTThoughtDefOf.BOT_ComedianGoodJokeOpinion);
				Pawn_InteractionsTracker.AddInteractionThought(initiator, recipient, BOTThoughtDefOf.BOT_ComedianGoodJokeOpinionSelf);
			}
			base.Interacted(initiator, recipient, extraSentencePacks, out letterText, out letterLabel, out letterDef, out lookTargets);
		}
    }
}
