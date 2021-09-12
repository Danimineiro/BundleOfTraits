using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using Verse.AI;
using UnityEngine;

namespace More_Traits
{
	class BOTTraitsManager : GameComponent
	{
		public static string src = "I made a lot of code in this looking at Vanilla Traits Expanded. Check out their mod at: https://steamcommunity.com/sharedfiles/filedetails/?id=2296404655";
		private static IntVec2 PyrophobeMinMaxFleeDistance = new IntVec2(12, 24);

		private static Dictionary<Pawn, int> MetabolismPawns;
		private static Dictionary<Pawn, Building_Bed> NyctophobesWhoCantSleep;
		private static Dictionary<Pawn, float> Loves_Sleep;
		private static Dictionary<Pawn, int> Narcoleptics;
		private static HashSet<Pawn> Pyrophobics;

		private List<Pawn> Nyctophobes = new List<Pawn>();
		private List<Building_Bed> NyctophobeBeds = new List<Building_Bed>();

		private List<Pawn> NarcolepticPawnKeys = new List<Pawn>();
		private List<int> NarcolepticPawnInts = new List<int>();

		private List<Pawn> MetabolismPawnKeys = new List<Pawn>();
		private List<int> MetabolismPawnInts = new List<int>();

		private List<Pawn> Loves_SleepPawnKeys = new List<Pawn>();
		private List<float> Loves_SleepInitialRestPercentage = new List<float>();

		static BOTTraitsManager()
		{
		}

		public BOTTraitsManager(Game game)
		{
		}

		public void PreInit()
		{
			if (Narcoleptics == null) Narcoleptics = new Dictionary<Pawn, int>();
			if (Pyrophobics == null) Pyrophobics = new HashSet<Pawn>();
			if (Loves_Sleep == null) Loves_Sleep = new Dictionary<Pawn, float>();
			if (NyctophobesWhoCantSleep == null) NyctophobesWhoCantSleep = new Dictionary<Pawn, Building_Bed>();
			if (MetabolismPawns == null) MetabolismPawns = new Dictionary<Pawn, int>();
		}

		public override void LoadedGame()
		{
			PreInit();
			base.LoadedGame();
		}

		public override void StartedNewGame()
		{
			PreInit();
			base.StartedNewGame();
		}

		public override void GameComponentTick()
		{
			base.GameComponentTick();
			if (GameTicksDivisibleBy(300))
			{
				Dictionary<Map, List<Thing>> MapFireDic = new Dictionary<Map, List<Thing>>();
				foreach (Pawn pawn in Pyrophobics)
				{
					if (pawn.Map != null && pawn.Map.fireWatcher.FireDanger > 0)
					{
						List<Thing> fires = null;

						if (MapFireDic.ContainsKey(pawn.Map))
						{
							fires = MapFireDic[pawn.Map];
						}
						else
						{
							fires = pawn.Map.listerThings.ThingsOfDef(ThingDefOf.Fire);
						}

						if (!pawn.Drafted)
						{
							if (fires != null && fires.Count != 0)
							{
								float closestFireDistance = fires.Min(fire => fire.Position.DistanceTo(pawn.Position));
								if (closestFireDistance < PyrophobeMinMaxFleeDistance.x)
								{
									BOTUtils.MakeFlee(pawn, fires.RandomElement(), PyrophobeMinMaxFleeDistance, fires);
									pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(BOTThoughtDefOf.BOT_PyrophobicNearFire, 1));
								}
							}
						}
						else if (pawn.Drafted)
						{
							if (fires != null && fires.Count != 0)
							{
								float closestFireDistance = fires.Min(fire => fire.Position.DistanceTo(pawn.Position));
								if (closestFireDistance < PyrophobeMinMaxFleeDistance.x)
								{
									pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(BOTThoughtDefOf.BOT_PyrophobicNearFire, 0));
								}
							}
						}
					}
				}
			}

			if (GameTicksDivisibleBy(2400))
			{
				foreach (KeyValuePair<Pawn, int> keyValuePair in MetabolismPawns)
				{
					//Get 25% more/less healing by running the healing part of the HealthTick function
					Pawn p = keyValuePair.Key;
					HediffSet hediffSet = p.health.hediffSet;

					//Heal fast Metabolism, damage flow by the same amount it was healed.
					if (p.RaceProps.IsFlesh && (p.needs.food == null || !p.needs.food.Starving))
					{
						bool flag2 = false;
						if (hediffSet.HasNaturallyHealingInjury())
						{
							float num = 8f;
							if (p.GetPosture() != PawnPosture.Standing)
							{
								num += 4f;
								Building_Bed building_Bed = p.CurrentBed();
								if (building_Bed != null)
								{
									num += building_Bed.def.building.bed_healPerDay;
								}
							}
							foreach (Hediff hediff3 in hediffSet.hediffs)
							{
								HediffStage curStage = hediff3.CurStage;
								if (curStage != null && curStage.naturalHealingFactor != -1f)
								{
									num *= curStage.naturalHealingFactor;
								}
							}

							if (p.story.traits.GetTrait(BOTTraitDefOf.BOT_Metabolism).Degree == -1)
							{
								(from x in hediffSet.GetHediffs<Hediff_Injury>()
								 where x.CanHealNaturally()
								 select x).RandomElement<Hediff_Injury>().Heal(num * p.HealthScale * 0.01f * p.GetStatValue(StatDefOf.InjuryHealingFactor, true));
								flag2 = true;
							} 
							else
							{
								(from x in hediffSet.GetHediffs<Hediff_Injury>()
								 where x.CanHealNaturally()
								 select x).RandomElement<Hediff_Injury>().InjureHediff(num * p.HealthScale * 0.01f * p.GetStatValue(StatDefOf.InjuryHealingFactor, true));

							}
						}

						if (hediffSet.HasTendedAndHealingInjury() && (p.needs.food == null || !p.needs.food.Starving))
						{
							Hediff_Injury hediff_Injury = (from x in hediffSet.GetHediffs<Hediff_Injury>()
															where x.CanHealFromTending()
															select x).RandomElement<Hediff_Injury>();
							float tendQuality = hediff_Injury.TryGetComp<HediffComp_TendDuration>().tendQuality;
							float num2 = GenMath.LerpDouble(0f, 1f, 0.5f, 1.5f, Mathf.Clamp01(tendQuality));

							if (p.story.traits.GetTrait(BOTTraitDefOf.BOT_Metabolism).Degree == -1)
							{
								hediff_Injury.Heal(8f * num2 * p.HealthScale * 0.01f * p.GetStatValue(StatDefOf.InjuryHealingFactor, true));
								flag2 = true;
							} else
							{
								hediff_Injury.InjureHediff(8f * num2 * p.HealthScale * 0.01f * p.GetStatValue(StatDefOf.InjuryHealingFactor, true));
							}
						}
						if (flag2 && !p.health.HasHediffsNeedingTendByPlayer(false) && !HealthAIUtility.ShouldSeekMedicalRest(p) && !p.health.hediffSet.HasTendedAndHealingInjury() && PawnUtility.ShouldSendNotificationAbout(p))
						{
							Messages.Message("MessageFullyHealed".Translate(p.LabelCap, p), p, MessageTypeDefOf.PositiveEvent, true);
						}
					}
				}
			}

			if (GameTicksDivisibleBy(1000))
			{
				//Narcoleptics
				HashSet<Pawn> reset = new HashSet<Pawn>();
				HashSet<Pawn> increment = new HashSet<Pawn>();
				foreach (KeyValuePair<Pawn, int> keyValuePair in Narcoleptics)
				{
					float sleepChance = 0.03125f;
					if (Narcoleptics[keyValuePair.Key] > 120000)
					{
						sleepChance = 0.25f;
					}
					else if (Narcoleptics[keyValuePair.Key] > 60000)
					{
						sleepChance = 0.125f;
					}
					else if (Narcoleptics[keyValuePair.Key] > 30000)
					{
						sleepChance = 0.0625f;
					}

					if (Narcoleptics[keyValuePair.Key] > 15000 && keyValuePair.Key.Spawned)
					{
						if (Rand.Value > sleepChance && (keyValuePair.Key.CurJob == null || keyValuePair.Key.CurJob.def != JobDefOf.LayDown))
						{
							keyValuePair.Key.jobs.StartJob(JobMaker.MakeJob(JobDefOf.LayDown, keyValuePair.Key.Position), JobCondition.InterruptForced, null, false, true, null, new JobTag?(JobTag.SatisfyingNeeds), false, false);
							if (keyValuePair.Key.InMentalState && keyValuePair.Key.MentalStateDef.recoverFromCollapsingExhausted)
							{
								keyValuePair.Key.mindState.mentalStateHandler.CurState.RecoverFromState();
							}
							if (PawnUtility.ShouldSendNotificationAbout(keyValuePair.Key))
							{
								Messages.Message("BOTNarcolepticInvoluntarySleep".Translate(keyValuePair.Key.LabelShort, keyValuePair.Key), keyValuePair.Key, MessageTypeDefOf.NegativeEvent, true);
							}
							reset.Add(keyValuePair.Key);
						}
					}
					else
					{
						if (!(keyValuePair.Key.jobs != null && keyValuePair.Key.jobs.curDriver != null && keyValuePair.Key.jobs.curDriver.asleep))
						{
							increment.Add(keyValuePair.Key);
						}
					}
				}

				foreach (Pawn p in reset)
				{
					Narcoleptics[p] = 0;
				}

				foreach (Pawn p in increment)
				{
					Narcoleptics[p] += 1000;
				}

				HashSet<Pawn> removeFromNycto = new HashSet<Pawn>();

				//Nyctophobes who can't sleep
				foreach (KeyValuePair<Pawn, Building_Bed> keyValuePair in NyctophobesWhoCantSleep)
				{
					IntVec3 bedPosition = keyValuePair.Value.Position;
					Map map = keyValuePair.Key.Map;

					if (bedPosition.InBounds(map) && bedPosition.Roofed(map) && map.glowGrid.GameGlowAt(bedPosition) < 0.3 && keyValuePair.Key.needs.rest.CurInstantLevelPercentage != 0 && keyValuePair.Key.CurJobDef == JobDefOf.LayDownAwake)
					{
						keyValuePair.Key.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(BOTThoughtDefOf.BOT_NyctophobiaCantSleep, 0));
					} else
					{
						removeFromNycto.Add(keyValuePair.Key);
						keyValuePair.Key.jobs.StartJob(JobMaker.MakeJob(JobDefOf.LayDown, keyValuePair.Value), JobCondition.InterruptForced, null, false, true, null, new JobTag?(JobTag.SatisfyingNeeds), false, false);
					}
				}

				foreach (Pawn p in removeFromNycto)
				{
					NyctophobesWhoCantSleep.Remove(p);
				}
			}

			if (GameTicksDivisibleBy(2000))
			{
				RemoveWrongPawnsFromSets();
			}
		}

		private bool GameTicksDivisibleBy(int n)
		{
			return (Find.TickManager.TicksGame % n == 0);
		}

		//This runs on game load when a pawn is spawned so PreInit should always get executed
		public void AddPawn(Pawn pawn)
		{
			PreInit();
			if (pawn.story != null && pawn.story.traits != null)
			{
				if (pawn.story.traits.HasTrait(BOTTraitDefOf.BOT_Narcoleptic))
				{
					Narcoleptics[pawn] = 0;
				}

				if (pawn.story.traits.HasTrait(BOTTraitDefOf.BOT_Pyrophobia))
				{
					Pyrophobics.Add(pawn);
				}

				if (pawn.story.traits.HasTrait(BOTTraitDefOf.BOT_Metabolism))
				{
					MetabolismPawns[pawn] = 0;
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look<Pawn, int>(ref Narcoleptics, "Narcoleptics", LookMode.Reference, LookMode.Value, ref NarcolepticPawnKeys, ref NarcolepticPawnInts);
			Scribe_Collections.Look<Pawn, float>(ref Loves_Sleep, "Loves_Sleep", LookMode.Reference, LookMode.Value, ref Loves_SleepPawnKeys, ref Loves_SleepInitialRestPercentage);
			Scribe_Collections.Look<Pawn>(ref Pyrophobics, "Pyrophobics", LookMode.Reference);
			Scribe_Collections.Look<Pawn, int>(ref MetabolismPawns, "MetabolismPawns", LookMode.Reference, LookMode.Value, ref MetabolismPawnKeys, ref MetabolismPawnInts);
			Scribe_Collections.Look<Pawn, Building_Bed>(ref NyctophobesWhoCantSleep, "NyctophobesWhoCantSleep", LookMode.Reference, LookMode.Reference, ref Nyctophobes, ref NyctophobeBeds);
		}

		public void RemoveWrongPawnsFromSets()
		{
			RemoveWrongPawnsFromDic(Narcoleptics, BOTTraitDefOf.BOT_Narcoleptic);
			RemoveWrongPawnsFromDic(Loves_Sleep, BOTTraitDefOf.BOT_Narcoleptic);
			RemoveWrongPawnsFromDic(MetabolismPawns, BOTTraitDefOf.BOT_Metabolism);
			RemoveWrongPawnsFromDic(NyctophobesWhoCantSleep, BOTTraitDefOf.BOT_Nyctophobia);

			Pyrophobics.RemoveWhere((Pawn p) => !p.story.traits.HasTrait(BOTTraitDefOf.BOT_Pyrophobia));
		}

		public void RemoveWrongPawnsFromDic(Dictionary<Pawn, int> dic, TraitDef traitDef)
		{
			List<Pawn> removePawns = new List<Pawn>();
			foreach (KeyValuePair<Pawn, int> keyValuePair in dic)
			{
				if (!keyValuePair.Key.story.traits.HasTrait(traitDef))
				{
					removePawns.Add(keyValuePair.Key);
				}
			}

			foreach (Pawn p in removePawns)
			{
				dic.Remove(p);
			}
		}

		public void RemoveWrongPawnsFromDic(Dictionary<Pawn, float> dic, TraitDef traitDef)
		{
			List<Pawn> removePawns = new List<Pawn>();
			foreach (KeyValuePair<Pawn, float> keyValuePair in dic)
			{
				if (!keyValuePair.Key.story.traits.HasTrait(traitDef))
				{
					removePawns.Add(keyValuePair.Key);
				}
			}

			foreach (Pawn p in removePawns)
			{
				dic.Remove(p);
			}
		}

		public void RemoveWrongPawnsFromDic(Dictionary<Pawn, Building_Bed> dic, TraitDef traitDef)
		{
			List<Pawn> removePawns = new List<Pawn>();
			foreach (KeyValuePair<Pawn, Building_Bed> keyValuePair in dic)
			{
				if (!keyValuePair.Key.story.traits.HasTrait(traitDef))
				{
					removePawns.Add(keyValuePair.Key);
				}
			}

			foreach (Pawn p in removePawns)
			{
				dic.Remove(p);
			}
		}

		public void RemoveDestroyedPawnFromSets(Pawn pawn)
		{
			Narcoleptics.Remove(pawn);
			Loves_Sleep.Remove(pawn);
			MetabolismPawns.Remove(pawn);
			NyctophobesWhoCantSleep.Remove(pawn);

			Pyrophobics.RemoveWhere((Pawn p) => p == pawn);
		}

		public Dictionary<Pawn, Building_Bed> GetNyctophobesWhoCantSleepDic()
		{
			return NyctophobesWhoCantSleep;
		}

		public Dictionary<Pawn, float> GetLoves_SleepDic()
		{
			return Loves_Sleep;
		}
	}

	[StaticConstructorOnStartup]
	class BOTTraitsPatcher
	{
		static BOTTraitsPatcher()
		{
			var harmony = new Harmony("dani.BOT.traits");
			harmony.PatchAll();
		}
	}

	[HarmonyPatch(typeof(TraitSet), "GainTrait")]
	public static class GainTraitPatch
	{
		public static void Postfix(Pawn ___pawn)
		{
			Current.Game.GetComponent<BOTTraitsManager>().AddPawn(___pawn);
		}
	}

	[HarmonyPatch(typeof(Pawn), "SpawnSetup")]
	public static class SpawnSetupPatch
	{
		public static void Postfix(Pawn __instance)
		{
			Current.Game.GetComponent<BOTTraitsManager>().AddPawn(__instance);
		}
	}

	[HarmonyPatch(typeof(Pawn), "Destroy")]
	public static class DestroyPatch
	{
		public static void Prefix(Pawn __instance)
		{
			Current.Game.GetComponent<BOTTraitsManager>().RemoveDestroyedPawnFromSets(__instance);
		}
	}

	//[HarmonyPatch(typeof(TraitSet), "GainTrait")]
	//public static class TraitConflictPatch
	//{
	//	public static void Postfix(TraitSet __instance, Trait trait)
	//	{
	//		if (trait.def == BOTTraitDefOf.Immunity || trait.def == BOTTraitDefOf.BOT_StrongBody)
 //         {
	//			if (__instance.HasTrait(BOTTraitDefOf.Immunity) && __instance.HasTrait(BOTTraitDefOf.BOT_StrongBody) && __instance.DegreeOfTrait(BOTTraitDefOf.Immunity) == -1)
 //               {
	//				__instance.RemoveTrait(trait);
	//				Log.Message("Removed " + trait.def.defName);
 //               }
 //           }
	//	}
	//}
}

