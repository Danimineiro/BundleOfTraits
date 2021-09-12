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
			var harmony = new Harmony("dani.BOT.thoughts");
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
							if (pawn2.story.traits.HasTrait(BOTTraitDefOf.BOT_Pacifist))
							{
								outIndividualThoughts.Add(new IndividualThoughtToAdd(BOTThoughtDefOf.BOT_WittnessedDeathPacifist, pawn2, null, 1, 1));

								if (pawn2 == (Pawn)dinfo.Value.Instigator)
								{
									outIndividualThoughts.Add(new IndividualThoughtToAdd(BOTThoughtDefOf.BOT_Pacifist_KilledHuman, pawn2, null, 1, 1));
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
					if (pawn2.story.traits.HasTrait(BOTTraitDefOf.BOT_Pacifist) && victim.RaceProps.Animal)
					{
						outIndividualThoughts.Add(new IndividualThoughtToAdd(BOTThoughtDefOf.BOT_Pacifist_KilledAnimal, pawn2, null, 1, 1));
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
				ingredients = (ingredients > BOTThoughtDefOf.BOT_EclecticPalateAte.stages.Count) ? BOTThoughtDefOf.BOT_EclecticPalateAte.stages.Count : ingredients;
				ingredients = (ingredients > 0) ? ingredients - 1 : 0;

				ingester.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(BOTThoughtDefOf.BOT_EclecticPalateAte, ingredients));
			}
		}
	}

	[HarmonyPatch(typeof(Pawn_JobTracker), "StartJob")]
	class StartJobPatch
	{
		public static void Prefix(Pawn ___pawn, Job newJob, JobCondition lastJobEndCondition)
		{
			if (___pawn.RaceProps.Animal) return;

			BOTTraitsManager Manager = Current.Game.GetComponent<BOTTraitsManager>();

			if (newJob.def == JobDefOf.LayDown && newJob.targetA.HasThing && newJob.targetA.Thing.GetType() == typeof(Building_Bed) && ___pawn.story != null && ___pawn.story.traits != null && ___pawn.story.traits.HasTrait(BOTTraitDefOf.BOT_Loves_Sleeping))
			{
				if (!Manager.GetLoves_SleepDic().ContainsKey(___pawn))
				{
					Manager.GetLoves_SleepDic()[___pawn] = ___pawn.needs.rest.CurLevelPercentage;
				}

				return;
			}

			//Only executed if the pawn was entered in here due to Job.LayDown in a Bed
			if (Manager.GetLoves_SleepDic().ContainsKey(___pawn))
			{
				float initialRestPercent = Manager.GetLoves_SleepDic()[___pawn];
				float currentRestPercent = ___pawn.needs.rest.CurLevelPercentage;
				float recreationGainPercent = (currentRestPercent - initialRestPercent) * 0.3f;

				___pawn.needs.joy.GainJoy(recreationGainPercent, BOTJoyKindDefOf.BOT_LovesSleepSleeping);

				if (currentRestPercent > 0.9)
				{
					___pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(BOTThoughtDefOf.BOT_LovesSleepWellRested, 0));
				}

				Manager.GetLoves_SleepDic().Remove(___pawn);
			}
		}
	}
}
