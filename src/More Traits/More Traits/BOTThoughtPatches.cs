using Verse;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;

namespace More_Traits
{
	[StaticConstructorOnStartup]
	class BOTThoughtPatches
	{
		static BOTThoughtPatches()
		{
			var harmony = new Harmony("dani.BOT.patch");
			harmony.PatchAll();
		}
	}

	[HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_ForHumanlike")]
	class PacifistWitnessDeathPatch
	{
		public static void Postfix(Pawn victim, DamageInfo? dinfo, PawnDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtToAddToAll> outAllColonistsThoughts)
		{
			bool flag = dinfo != null && dinfo.Value.Def.execution;
			if (thoughtsKind == PawnDiedOrDownedThoughtsKind.Died && !flag)
			{
				foreach (Pawn pawn2 in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive)
				{
					if (pawn2 != victim && pawn2.needs != null && pawn2.needs.mood != null && (pawn2.MentalStateDef != MentalStateDefOf.SocialFighting || ((MentalState_SocialFighting)pawn2.MentalState).otherPawn != victim))
					{
						if (ThoughtUtility.Witnessed(pawn2, victim))
						{
							if (pawn2.story.traits.HasTrait(BOTDefOf.BOT_Pacifist))
							{
								outIndividualThoughts.Add(new IndividualThoughtToAdd(BOTDefOf.BOT_WittnessedDeathPacifist, pawn2, null, 1, 1));

								if (pawn2 == (Pawn)dinfo.Value.Instigator)
								{
									outIndividualThoughts.Add(new IndividualThoughtToAdd(BOTDefOf.BOT_Pacifist_KilledHuman, pawn2, null, 1, 1));
								}
							}
						}
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_Relations")]
	class PacifistKilledPatch
	{
		public static void Postfix (Pawn victim, DamageInfo? dinfo, PawnDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtToAddToAll> outAllColonistsThoughts)
		{
			if (dinfo != null && thoughtsKind == PawnDiedOrDownedThoughtsKind.Died)
			{
                if (dinfo.Value.Instigator is Pawn pawn2 && pawn2 != victim && victim.needs != null)
                {
                    if (pawn2.story.traits.HasTrait(BOTDefOf.BOT_Pacifist) && victim.RaceProps.Animal)
                    {
                        outIndividualThoughts.Add(new IndividualThoughtToAdd(BOTDefOf.BOT_Pacifist_KilledAnimal, pawn2, null, 1, 1));
                    }
                }
            }
		}
	}

	[HarmonyPatch(typeof(Toils_Ingest), "FinalizeIngest")]
	class EclecticIngestPatch
	{
		public static void Postfix(Pawn ingester, TargetIndex ingestibleInd)
		{
			Thing food = ingester.CurJob.GetTarget(ingestibleInd).Thing;
			bool isMeal = food.HasThingCategory(ThingCategoryDefOf.FoodMeals);

			if (ingester.needs.mood != null && isMeal)
            {
				int ingredients = food.TryGetComp<CompIngredients>().ingredients.Count;
				ingredients = (ingredients > BOTDefOf.BOT_EclecticPalateAte.stages.Count) ? BOTDefOf.BOT_EclecticPalateAte.stages.Count : ingredients;
				ingredients = (ingredients > 0) ? ingredients - 1 : 0;

				ingester.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(BOTDefOf.BOT_EclecticPalateAte, ingredients));
			}
		}
	}
}
