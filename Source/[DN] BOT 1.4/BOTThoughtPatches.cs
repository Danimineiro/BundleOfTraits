using Verse;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;
using System;
using UnityEngine;
using RimWorld.Planet;

namespace More_Traits
{
	[StaticConstructorOnStartup]
	class BOTPatcher
	{
		static BOTPatcher()
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
			bool flag = dinfo.HasValue && dinfo?.Def.execution == true;

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

                                if (pawn2 == (Pawn)dinfo?.Instigator)
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
		public static void Postfix(Pawn victim, DamageInfo? dinfo, PawnDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtToAddToAll> outAllColonistsThoughts)
		{
			if (dinfo != null && thoughtsKind == PawnDiedOrDownedThoughtsKind.Died)
			{
				if (dinfo.Value.Instigator is Pawn pawn2 && pawn2 != victim && victim.needs != null && !pawn2.RaceProps.Animal)
				{
					//Adding null checks to avoid Mechanoids from processing any further - they don't have these fields afaik
					if (pawn2.story?.traits?.HasTrait(BOTTraitDefOf.BOT_Pacifist) == true && victim.RaceProps.Animal)
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

			if (food == null) return;

			bool isMeal = food.HasThingCategory(ThingCategoryDefOf.FoodMeals);


			AddThoughtsClass finisher = new AddThoughtsClass(food, isMeal, ingester);
			Action actionTarget = finisher.AddThoughts;

			__result.AddFinishAction(actionTarget);
		}

		private class AddThoughtsClass
		{
			private readonly Thing food;
			private readonly bool isMeal;
			private readonly Pawn ingester;

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
	///		This class manages Communal pawn thoughts
	/// </summary>
	[HarmonyPatch(typeof(Toils_LayDown), "ApplyBedThoughts")]
	class BedPatch
	{
		public static void Postfix(Pawn actor)
		{
			if (actor.needs.mood == null || !actor.HasTrait(BOTTraitDefOf.BOT_Communal)) return;

			Building_Bed building_Bed = actor.CurrentBed();

			actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(BOTThoughtDefOf.BOT_Communal_SleptInBarracks);
			actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(BOTThoughtDefOf.BOT_Communal_SleptInBedroom);

			if (actor.GetRoom(RegionType.Set_All).PsychologicallyOutdoors || building_Bed == null || building_Bed.CostListAdjusted().Count == 0) return;

			if (building_Bed != null && building_Bed == actor.ownership.OwnedBed && !building_Bed.ForPrisoners && !actor.HasTrait(TraitDefOf.Ascetic))
            {
                ThoughtDef thoughtDef = null;
                if (building_Bed.GetRoom(RegionType.Set_All).Role == RoomRoleDefOf.Bedroom)
                {
                    thoughtDef = BOTThoughtDefOf.BOT_Communal_SleptInBedroom;
                }
                else if (building_Bed.GetRoom(RegionType.Set_All).Role == RoomRoleDefOf.Barracks)
                {
                    thoughtDef = BOTThoughtDefOf.BOT_Communal_SleptInBarracks;
                }
                if (thoughtDef == null) return;

                int scoreStageIndex = RoomStatDefOf.Impressiveness.GetScoreStageIndex(building_Bed.GetRoom(RegionType.Set_All).GetStat(RoomStatDefOf.Impressiveness));
				if (thoughtDef.stages[scoreStageIndex] == null) return;

                int owners = -1;
                foreach (Building_Bed bed in building_Bed.GetRoom(RegionType.Set_All).ContainedBeds) owners += bed.OwnersForReading.Count;

                int nrOfOthers = Mathf.Clamp(owners, 0, 20);

                actor.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(BOTThoughtDefOf.BOT_CommunalSharing, BOTUtils.StageOfTwenty(nrOfOthers)));
                actor.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(thoughtDef, scoreStageIndex));
            }
        }
	}

	[HarmonyPatch(typeof(ThoughtUtility), "RemovePositiveBedroomThoughts")]
	class RemovePositiveBedroomThoughtsPatch
	{
		public static void Postfix(Pawn pawn)
		{
			if (pawn.needs.mood == null)
			{
				return;
			}

			pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefIf(BOTThoughtDefOf.BOT_Communal_SleptInBarracks, (Thought_Memory thought) => thought.MoodOffset() > 0f);
			pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefIf(BOTThoughtDefOf.BOT_Communal_SleptInBedroom, (Thought_Memory thought) => thought.MoodOffset() > 0f);
			pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefIf(BOTThoughtDefOf.BOT_CommunalSharing, (Thought_Memory thought) => thought.MoodOffset() > 0f);
		}
	}

	/// <summary>
	///		This class manages Sadist thoughts
	/// </summary>
	[HarmonyPatch(typeof(Pawn_StanceTracker), "Notify_DamageTaken")]
	class Notify_DamageTakenPatch
	{
		public static bool tag = false;

		public static void Postfix(Pawn_StanceTracker __instance, DamageInfo dinfo)
		{
			Pawn victim = __instance.pawn;
			Pawn instigator = dinfo.Instigator as Pawn;

			if (dinfo.Def == DamageDefOf.SurgicalCut)
            {
				tag = true;
				return;
            }

			if (dinfo.Def == DamageDefOf.ExecutionCut)
            {
				return;
            }

			if (instigator.HasTrait(BOTTraitDefOf.BOT_Sadist) && instigator.needs.mood != null)
			{
				instigator.needs.mood.thoughts.memories.TryGainMemory(BOTThoughtDefOf.BOT_HurtHumanlikeSadist);
			}

			foreach (Pawn pawn in victim.Map.mapPawns.AllPawnsSpawned.Where(thePawn => thePawn.HasTrait(BOTTraitDefOf.BOT_Sadist) && thePawn.needs.mood != null && thePawn != instigator && thePawn != victim && instigator != null))
			{
				bool flag = false;

				try
                {
					flag = ThoughtUtility.Witnessed(pawn, instigator);
				} 
				catch
                {
					Log.Warning("[Bundle of Traits] Some kind of damage could have caused major errors here: " + dinfo.ToString() + " You can ignore this warning but it would be nice if you could send the BOT dev this log file.");
                }

				if (flag) pawn.needs.mood.thoughts.memories.TryGainMemory(BOTThoughtDefOf.BOT_WitnessedDamageSadist);
			}
		}
	}

	[HarmonyPatch(typeof(Recipe_RemoveBodyPart), "ApplyOnPawn")]
	class ApplyOnPawnPatch
	{
		public static void Prefix()
		{
			Notify_DamageTakenPatch.tag = false;
		}

		public static void Postfix(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			if (billDoer.needs.mood != null && billDoer.HasTrait(BOTTraitDefOf.BOT_Sadist) && Notify_DamageTakenPatch.tag)
			{
				billDoer.needs.mood.thoughts.memories.TryGainMemory(BOTThoughtDefOf.BOT_HarvestedOrgan_Sadist);
			}
			Notify_DamageTakenPatch.tag = false;
		}
	}
}
