using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using Verse.AI;
using UnityEngine;

namespace More_Traits
{
	/// <summary>
	///		This class is used to store information regarding traits added by this mod and to manage trait behaviour.
	///		A lot of what is seen here is based on Vanilla Traits expanded code
	/// </summary>
	class BOTTraitsManager : GameComponent
	{
		public static string src = "I made a lot of code in this looking at Vanilla Traits Expanded. Check out their mod at: https://steamcommunity.com/sharedfiles/filedetails/?id=2296404655";
		private static IntVec2 PyrophobeMinMaxFleeDistance = new IntVec2(12, 24);

		private static Dictionary<Pawn, int> MetabolismPawns;
		private static Dictionary<Pawn, Building_Bed> NyctophobesWhoCantSleep;
		private static Dictionary<Pawn, float> Loves_Sleep;
		private static Dictionary<Pawn, int> Narcoleptics;
		private static HashSet<Pawn> Pyrophobics;
		private static HashSet<Pawn> SleepyHeads;

		private List<Pawn> Nyctophobes = new List<Pawn>();
		private List<Building_Bed> NyctophobeBeds = new List<Building_Bed>();

		private List<Pawn> NarcolepticPawnKeys = new List<Pawn>();
		private List<int> NarcolepticPawnInts = new List<int>();

		private List<Pawn> MetabolismPawnKeys = new List<Pawn>();
		private List<int> MetabolismPawnInts = new List<int>();

		private List<Pawn> Loves_SleepPawnKeys = new List<Pawn>();
		private List<float> Loves_SleepInitialRestPercentage = new List<float>();

		//I don't actually know if these constructors are requiered, but they don't really do harm being here anyways
		static BOTTraitsManager()
		{
		}

		public BOTTraitsManager(Game game)
		{
		}

		/// <summary>
		///		This function checks if the dictionaries and hashsets used to store variables in this mod are initialized, and if not, does so
		/// </summary>
		public void PreInit()
		{
			if (Narcoleptics == null) Narcoleptics = new Dictionary<Pawn, int>();
			if (Pyrophobics == null) Pyrophobics = new HashSet<Pawn>();
			if (SleepyHeads == null) SleepyHeads = new HashSet<Pawn>();
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

		/// <summary>
		///		This function manages the behaviour of traits
		/// </summary>
		public override void GameComponentTick()
		{
			base.GameComponentTick();

			ManagePyrophobes(300);
			ManageSleepyHeads(500);
			ManageNarcoleptics(1000);
			ManageMetabolism(2400);

			RemoveWrongPawnsFromSets(2000);
		}

		/// <summary>
		///		Determines if the current game tick is divisible by n
		/// </summary>
		/// <param name="n">the divisor</param>
		/// <returns>true if the game tick is divisible by n, false otherwise</returns>
		private bool GameTicksDivisibleBy(int n)
		{
			return (Find.TickManager.TicksGame % n == 0);
		}

		private void ManagePyrophobes(int whenTicksDivisibleBy)
		{
			if (GameTicksDivisibleBy(whenTicksDivisibleBy))
			{
				Dictionary<Map, Dictionary<Thing, float>> MapFireDic = new Dictionary<Map, Dictionary<Thing, float>>();
				foreach (Pawn pawn in Pyrophobics)
				{
					if (pawn.Map != null && pawn.Map.fireWatcher.FireDanger > 0)
					{
						Dictionary<Thing, float> fires = new Dictionary<Thing, float>();
						Thing closestFire = null;
						float closestFireDistance = float.MaxValue;
						bool hasLOS = false;

						if (MapFireDic.ContainsKey(pawn.Map))
						{
							fires = MapFireDic[pawn.Map];
						}
						else
						{
							foreach (Thing fire in pawn.Map.listerThings.ThingsOfDef(ThingDefOf.Fire))
                            {
								float fireDistance = fire.Position.DistanceTo(pawn.Position);
								fires[fire] = fireDistance;
								hasLOS = hasLOS ? hasLOS : GenSight.LineOfSight(pawn.Position, fire.Position, pawn.Map);

								if (fireDistance < closestFireDistance)
                                {
									closestFireDistance = fireDistance;
									closestFire = fire;
                                }
                            }
						}

						if (!pawn.Drafted && hasLOS)
						{
							if (fires != null && fires.Count != 0)
							{
								if (closestFireDistance < PyrophobeMinMaxFleeDistance.x)
								{
									pawn.MakeFlee(closestFire, PyrophobeMinMaxFleeDistance, fires.Keys.ToList());
									pawn.TryGainMemory(BOTThoughtDefOf.BOT_PyrophobicNearFire, 1);
								}
							}
						}
						else if (pawn.Drafted && hasLOS)
						{
							if (fires != null && fires.Count != 0)
							{
								if (closestFireDistance < PyrophobeMinMaxFleeDistance.x)
								{
									pawn.TryGainMemory(BOTThoughtDefOf.BOT_PyrophobicNearFire, 0);
								}
							}
						}
					}
				}
			}
		}

		private void ManageMetabolism(int whenTicksDivisibleBy)
		{
			if (GameTicksDivisibleBy(whenTicksDivisibleBy))
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
								 select x).RandomElement<Hediff_Injury>().Injure(num * p.HealthScale * 0.01f * p.GetStatValue(StatDefOf.InjuryHealingFactor, true));

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
							}
							else
							{
								hediff_Injury.Injure(8f * num2 * p.HealthScale * 0.01f * p.GetStatValue(StatDefOf.InjuryHealingFactor, true));
							}
						}
						if (flag2 && !p.health.HasHediffsNeedingTendByPlayer(false) && !HealthAIUtility.ShouldSeekMedicalRest(p) && !p.health.hediffSet.HasTendedAndHealingInjury() && PawnUtility.ShouldSendNotificationAbout(p))
						{
							Messages.Message("MessageFullyHealed".Translate(p.LabelCap, p), p, MessageTypeDefOf.PositiveEvent, true);
						}
					}
				}
			}
		}

		private void ManageNarcoleptics(int whenTicksDivisibleBy)
		{
			if (GameTicksDivisibleBy(whenTicksDivisibleBy))
			{
				//Narcoleptics
				HashSet<Pawn> reset = new HashSet<Pawn>();
				HashSet<Pawn> increment = new HashSet<Pawn>();
				foreach (KeyValuePair<Pawn, int> keyValuePair in Narcoleptics)
				{
					float baseSleepChance = 0.015625f;
					float sleepChance = baseSleepChance;
					if (Narcoleptics[keyValuePair.Key] > 120000)
					{
						sleepChance *= 8f;
					}
					else if (Narcoleptics[keyValuePair.Key] > 60000)
					{
						sleepChance *= 4f;
					}
					else if (Narcoleptics[keyValuePair.Key] > 30000)
					{
						sleepChance *= 2f;
					}

					if (Narcoleptics[keyValuePair.Key] > 15000 && keyValuePair.Key.Spawned)
					{
						if (Rand.Value < sleepChance && (keyValuePair.Key.CurJob == null || keyValuePair.Key.CurJob.def != JobDefOf.LayDown))
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

					if (bedPosition.InBounds(map) && bedPosition.Roofed(map) && map.glowGrid.GameGlowAt(bedPosition) < 0.3 && keyValuePair.Key.needs.rest.CurCategory != RestCategory.Exhausted && keyValuePair.Key.CurJobDef == JobDefOf.LayDownAwake)
					{
						keyValuePair.Key.TryGainMemory(BOTThoughtDefOf.BOT_NyctophobiaCantSleep, 0);
					}
					else
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
		}

		private void ManageSleepyHeads(int whenTicksDivisibleBy)
		{
			if (GameTicksDivisibleBy(whenTicksDivisibleBy))
			{
				List<Pawn> toRemove = new List<Pawn>();
				foreach(Pawn p in SleepyHeads)
				{
					if (Rand.Value > 0.1 && p.CurJobDef == JobDefOf.LayDown)
					{
						p.TryGainMemory(BOTThoughtDefOf.BOT_SleepyHeadStopsSleeping, 0);
						p.jobs.EndCurrentJob(JobCondition.Succeeded);
						toRemove.Add(p);
					} 
					//Might got drafted or removed from the bed for other reasons
					else if (p.CurJobDef != JobDefOf.LayDown)
                    {
						p.TryGainMemory(BOTThoughtDefOf.BOT_SleepyHeadForcefullyWokenUp, 0);
						toRemove.Add(p);
                    } else
                    {
						p.TryGainMemory(BOTThoughtDefOf.BOT_SleepyHeadContinuesSleeping, 0);
                    }
				}

				foreach(Pawn p in toRemove)
                {
					SleepyHeads.Remove(p);
					p.needs.rest.CurLevelPercentage = 1f;
				}
			}
		}

		//This runs on game load when a pawn is spawned so PreInit should always get executed
		/// <summary>
		///		Adds a pawn to the correct list
		/// </summary>
		/// <param name="pawn">the pawn to be added</param>
		public void AddPawn(Pawn pawn)
		{
			PreInit();
			if (pawn.story != null && pawn.story.traits != null)
			{
				if (pawn.story.traits.HasTrait(BOTTraitDefOf.BOT_Narcoleptic))
				{
					if (!Narcoleptics.ContainsKey(pawn))
					{
						Narcoleptics[pawn] = 0; 
					}
				}

				if (pawn.story.traits.HasTrait(BOTTraitDefOf.BOT_Pyrophobia))
				{
					Pyrophobics.Add(pawn);
				}

				if (pawn.story.traits.HasTrait(BOTTraitDefOf.BOT_Metabolism))
				{
					if (!MetabolismPawns.ContainsKey(pawn))
					{
						MetabolismPawns[pawn] = 0;
					}
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look<Pawn, int>(ref Narcoleptics, "Narcoleptics", LookMode.Reference, LookMode.Value, ref NarcolepticPawnKeys, ref NarcolepticPawnInts);
			Scribe_Collections.Look<Pawn, float>(ref Loves_Sleep, "Loves_Sleep", LookMode.Reference, LookMode.Value, ref Loves_SleepPawnKeys, ref Loves_SleepInitialRestPercentage);
			Scribe_Collections.Look<Pawn>(ref Pyrophobics, "Pyrophobics", LookMode.Reference);
			Scribe_Collections.Look<Pawn>(ref SleepyHeads, "SleepyHeads", LookMode.Reference);
			Scribe_Collections.Look<Pawn, int>(ref MetabolismPawns, "MetabolismPawns", LookMode.Reference, LookMode.Value, ref MetabolismPawnKeys, ref MetabolismPawnInts);
			Scribe_Collections.Look<Pawn, Building_Bed>(ref NyctophobesWhoCantSleep, "NyctophobesWhoCantSleep", LookMode.Reference, LookMode.Reference, ref Nyctophobes, ref NyctophobeBeds);
		}

		/// <summary>
		///		Checks if the pawns in the dictionaries and hashlists should be inside them
		/// </summary>
		/// <param name="whenTicksDivisibleBy"></param>
		public void RemoveWrongPawnsFromSets(int whenTicksDivisibleBy)
		{
			if (GameTicksDivisibleBy(whenTicksDivisibleBy))
			{
				RemoveWrongPawnsFromDic(Narcoleptics, BOTTraitDefOf.BOT_Narcoleptic);
				RemoveWrongPawnsFromDic(Loves_Sleep, BOTTraitDefOf.BOT_Narcoleptic);
				RemoveWrongPawnsFromDic(MetabolismPawns, BOTTraitDefOf.BOT_Metabolism);
	
				Pyrophobics.RemoveWhere((Pawn p) => !p.story.traits.HasTrait(BOTTraitDefOf.BOT_Pyrophobia));
			}
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

		/// <summary>
		///		Removes a dead pawn from any lists/dictionaries/hashSets
		/// </summary>
		/// <param name="pawn"></param>
		public void RemoveDestroyedPawnFromSets(Pawn pawn)
		{
			Narcoleptics.Remove(pawn);
			Loves_Sleep.Remove(pawn);
			MetabolismPawns.Remove(pawn);
			NyctophobesWhoCantSleep.Remove(pawn);

			SleepyHeads.Remove(pawn);
			Pyrophobics.Remove(pawn);
		}

		public Dictionary<Pawn, Building_Bed> GetNyctophobesWhoCantSleepDic()
		{
			return NyctophobesWhoCantSleep;
		}

		public Dictionary<Pawn, float> GetLoves_SleepDic()
		{
			return Loves_Sleep;
		}

		public HashSet<Pawn> GetSleepyHeadSet()
		{
			return SleepyHeads;
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
	//      {
	//			if (__instance.HasTrait(BOTTraitDefOf.Immunity) && __instance.HasTrait(BOTTraitDefOf.BOT_StrongBody) && __instance.DegreeOfTrait(BOTTraitDefOf.Immunity) == -1)
	//               {
	//				__instance.RemoveTrait(trait);
	//				Log.Message("Removed " + trait.def.defName);
	//               }
	//           }
	//	}
	//}
}

