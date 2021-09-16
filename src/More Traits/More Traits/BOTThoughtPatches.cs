using Verse;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;
using System;

namespace More_Traits
{
	[StaticConstructorOnStartup]
	class BOTPatches
	{
		static BOTPatches()
		{
			var harmony = new Harmony("dani.BOT.Patches");
			harmony.PatchAll();

		}
	}

	/// <summary>
	///		This class manages the adding of some death related thoughts to pacifists
	/// </summary>
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
								outIndividualThoughts.Add(new IndividualThoughtToAdd(BOTThoughtDefOf.BOT_WittnessedDeathPacifist, pawn2, null));

								if (pawn2 == (Pawn)dinfo.Value.Instigator)
								{
									outIndividualThoughts.Add(new IndividualThoughtToAdd(BOTThoughtDefOf.BOT_Pacifist_KilledHuman, pawn2, null));
								}
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	///		This class manages the adding of some death related thoughts to pacifists
	/// </summary>
	[HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_Relations")]
	class PacifistKilledPatch
	{
		public static void Postfix (Pawn victim, DamageInfo? dinfo, PawnDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtToAddToAll> outAllColonistsThoughts)
		{
			if (dinfo != null && thoughtsKind == PawnDiedOrDownedThoughtsKind.Died)
			{
				if (dinfo.Value.Instigator is Pawn pawn2 && pawn2 != victim && victim.needs != null && !pawn2.RaceProps.Animal)
				{
					if (pawn2.story.traits.HasTrait(BOTTraitDefOf.BOT_Pacifist) && victim.RaceProps.Animal)
					{
						outIndividualThoughts.Add(new IndividualThoughtToAdd(BOTThoughtDefOf.BOT_Pacifist_KilledAnimal, pawn2, null));
					}
				}
			}
		}
	}

	/// <summary>
	///		This class manages the adding of some food related thoughts to eclectics
	/// </summary>
	[HarmonyPatch(typeof(Toils_Ingest), "FinalizeIngest")]
	class IngestPatch
	{
		public static void Postfix(Pawn ingester, TargetIndex ingestibleInd, ref Toil __result)
		{
			Thing food = ingester.CurJob.GetTarget(ingestibleInd).Thing;
			bool isMeal = food.HasThingCategory(ThingCategoryDefOf.FoodMeals);


			AddThoughtsClass finisher = new AddThoughtsClass(food, isMeal, ingester);
			Action actionTarget = finisher.AddThoughts;

			__result.AddFinishAction(actionTarget);
		}

		private class AddThoughtsClass
        {
			Thing food;
			bool isMeal;
			Pawn ingester;

			public AddThoughtsClass(Thing food, bool isMeal, Pawn ingester)
            {
				this.food = food;
				this.isMeal = isMeal;
				this.ingester = ingester;
            }

			public void AddThoughts()
			{
				if (ingester.needs.mood != null && isMeal)
				{

					List<ThingDef> ingredients = food.TryGetComp<CompIngredients>().ingredients;
					int nrOfIngredients = ingredients.Count;
					nrOfIngredients = (nrOfIngredients > BOTThoughtDefOf.BOT_EclecticPalateAte.stages.Count) ? BOTThoughtDefOf.BOT_EclecticPalateAte.stages.Count : nrOfIngredients;
					nrOfIngredients = (nrOfIngredients > 0) ? nrOfIngredients - 1 : 0;

					ingester.TryGainMemory(BOTThoughtDefOf.BOT_EclecticPalateAte, nrOfIngredients);
				}
			}
		}
	}

	/// <summary>
	///		This class patches the startjob function to modify the behaviour of sleep related traits
	/// </summary>
	[HarmonyPatch(typeof(Pawn_JobTracker), "StartJob")]
	class StartJobPatch
	{
		public static void Prefix(Pawn ___pawn, Job newJob, JobCondition lastJobEndCondition)
		{
			if (___pawn.RaceProps.Animal || ___pawn.story == null || ___pawn.story.traits == null) return;

			BOTTraitsManager Manager = Current.Game.GetComponent<BOTTraitsManager>();

			//Determine if a Nyctophobic person can sleep
			if (newJob.def == JobDefOf.LayDown && newJob.targetA.HasThing && newJob.targetA.Thing.GetType() == typeof(Building_Bed) && ___pawn.story.traits.HasTrait(BOTTraitDefOf.BOT_Nyctophobia))
			{
				IntVec3 bedPosition = newJob.targetA.Thing.Position;
				Map map = ___pawn.Map;

				//Exhaustion will eventually start sleeping jobs which is very buggy when these aren't accepted by my check, so exhausted pawns can always sleep
				if (bedPosition.InBounds(map) && bedPosition.Roofed(map) && map.glowGrid.GameGlowAt(bedPosition) < 0.3 && ___pawn.needs.rest.CurCategory != RestCategory.Exhausted)
				{
					newJob.def = JobDefOf.LayDownAwake;
					___pawn.TryGainMemory(BOTThoughtDefOf.BOT_NyctophobiaCantSleep, 0);
					Messages.Message("BOTNyctophobeCantSleep".Translate(___pawn.LabelShort, ___pawn), ___pawn, MessageTypeDefOf.NegativeEvent, true);
					Manager.GetNyctophobesWhoCantSleepDic()[___pawn] = (Building_Bed) newJob.targetA.Thing;
				}
			}

			//Save the amount of rest a pawn with the Loves_Sleep trait has in order to later calculate how much recreation they should gain from that
			if (newJob.def == JobDefOf.LayDown && newJob.targetA.HasThing && newJob.targetA.Thing.GetType() == typeof(Building_Bed) && ___pawn.story.traits.HasTrait(BOTTraitDefOf.BOT_Loves_Sleeping))
			{
				if (!Manager.GetLoves_SleepDic().ContainsKey(___pawn))
				{
					Manager.GetLoves_SleepDic()[___pawn] = ___pawn.needs.rest.CurLevelPercentage;
				}

				return;
			}

			//Only executed if the pawn was entered in here due to Job.LayDown in a Bed
			//determines how much recreation a pawn should get
			if (Manager.GetLoves_SleepDic().ContainsKey(___pawn))
			{
				float initialRestPercent = Manager.GetLoves_SleepDic()[___pawn];
				float currentRestPercent = ___pawn.needs.rest.CurLevelPercentage;
				float recreationGainPercent = (currentRestPercent - initialRestPercent) * 0.3f;

				___pawn.needs.joy.GainJoy(recreationGainPercent, BOTJoyKindDefOf.BOT_LovesSleepSleeping);

				if (currentRestPercent > 0.9)
				{
					___pawn.TryGainMemory(BOTThoughtDefOf.BOT_LovesSleepWellRested, 0);
				}

				Manager.GetLoves_SleepDic().Remove(___pawn);
			}

			//Deals with sleepyheads not wanting to wake up
			if ((___pawn.CurJobDef == JobDefOf.LayDown || ___pawn.CurJobDef == JobDefOf.LayDownAwake) && newJob.def != JobDefOf.LayDown && ___pawn.story.traits.HasTrait(BOTTraitDefOf.BOT_Sleepyhead) && ___pawn.needs.rest.CurCategory == RestCategory.Rested && !Manager.GetSleepyHeadSet().Contains(___pawn)) 
			{
				if (Rand.Value > 0.3)
				{
					newJob = ___pawn.CurJob;
					newJob.def = JobDefOf.LayDown;
					newJob.forceSleep = true;
					___pawn.needs.rest.CurLevelPercentage = 0.30f;
					___pawn.TryGainMemory(BOTThoughtDefOf.BOT_SleepyHeadContinuesSleeping, 0);
					Manager.GetSleepyHeadSet().Add(___pawn);
				}
			}
		}
	}
}
