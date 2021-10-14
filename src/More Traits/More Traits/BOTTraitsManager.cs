using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using Verse.AI;
using UnityEngine;
using RimWorld.Planet;

namespace More_Traits
{
	/// <summary>
	///		This class is used to store information regarding traits added by this mod and to manage trait behaviour.
	///		A lot of what is seen here is based on Vanilla Traits expanded code
	/// </summary>
	class BOTTraitsManager : GameComponent
	{
		public static string src = "I made a lot of code in this looking at Vanilla Traits Expanded. Check out their mod at: https://steamcommunity.com/sharedfiles/filedetails/?id=2296404655";
		private static IntVec2 MinMaxFleeDistance = new IntVec2(12, 24);

		private static HashSet<Pawn> EntomophobicPawns;
		private static Dictionary<Pawn, int> MetabolismPawns;
		private static HashSet<Pawn> Nyctophobes;
		private static Dictionary<Pawn, float> Loves_SleepPawns;
		private static Dictionary<Pawn, int> NarcolepticPawns;
		private static HashSet<Pawn> PyrophobicPawns;
		private static HashSet<Pawn> SleepyHeadPawns;

		private List<Pawn> NarcolepticPawnKeys = new List<Pawn>();
		private List<int> NarcolepticPawnInts = new List<int>();

		private List<Pawn> MetabolismPawnKeys = new List<Pawn>();
		private List<int> MetabolismPawnInts = new List<int>();

		private List<Pawn> Loves_SleepPawnKeys = new List<Pawn>();
		private List<float> Loves_SleepInitialRestPercentage = new List<float>();

		private readonly int fleeIntervallInTicks = 200;

		//I don't actually know if these constructors are requiered, but they don't really do harm being here anyways
		static BOTTraitsManager()
		{
		}

		public BOTTraitsManager(Game _)
		{
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look<Pawn, int>(ref NarcolepticPawns, "Narcoleptics", LookMode.Reference, LookMode.Value, ref NarcolepticPawnKeys, ref NarcolepticPawnInts);
			Scribe_Collections.Look<Pawn, float>(ref Loves_SleepPawns, "Loves_Sleep", LookMode.Reference, LookMode.Value, ref Loves_SleepPawnKeys, ref Loves_SleepInitialRestPercentage);
			Scribe_Collections.Look<Pawn>(ref PyrophobicPawns, "Pyrophobics", LookMode.Reference);
			Scribe_Collections.Look<Pawn>(ref SleepyHeadPawns, "SleepyHeads", LookMode.Reference);
			Scribe_Collections.Look<Pawn>(ref EntomophobicPawns, "Entomophobics", LookMode.Reference);
			Scribe_Collections.Look<Pawn, int>(ref MetabolismPawns, "MetabolismPawns", LookMode.Reference, LookMode.Value, ref MetabolismPawnKeys, ref MetabolismPawnInts);
			Scribe_Collections.Look<Pawn>(ref Nyctophobes, "Nyctophobes", LookMode.Reference);
		}

		/// <summary>
		///		This function checks if the dictionaries and hashsets used to store variables in this mod are initialized, and if not, does so
		/// </summary>
		public void PreInit()
		{
			if (NarcolepticPawns == null) NarcolepticPawns = new Dictionary<Pawn, int>();
			if (PyrophobicPawns == null) PyrophobicPawns = new HashSet<Pawn>();
			if (SleepyHeadPawns == null) SleepyHeadPawns = new HashSet<Pawn>();
			if (Loves_SleepPawns == null) Loves_SleepPawns = new Dictionary<Pawn, float>();
			if (Nyctophobes == null) Nyctophobes = new HashSet<Pawn>();
			if (MetabolismPawns == null) MetabolismPawns = new Dictionary<Pawn, int>();
			if (EntomophobicPawns == null) EntomophobicPawns = new HashSet<Pawn>();

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

			ManageFleeing(fleeIntervallInTicks, PyrophobicPawns, PyrophobicPawns.Any(pawn => pawn.Map != null && pawn.Map.fireWatcher.FireDanger > 0));
			ManageFleeing(fleeIntervallInTicks, EntomophobicPawns, EntomophobicPawns.Any(pawn => pawn.Map != null && pawn.Map.mapPawns.AllPawnsSpawned.Exists(x => x.def.devNote == "insect" || x.def.race.FleshType == FleshTypeDefOf.Insectoid)), true);
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

		private void ManageFleeing(int whenTicksDivisibleBy, HashSet<Pawn> hashSet, bool underCondition, bool alsoWhenDrafted = false)
		{
			if (GameTicksDivisibleBy(whenTicksDivisibleBy))
			{
				Dictionary<Map, Dictionary<Thing, float>> MapDic = new Dictionary<Map, Dictionary<Thing, float>>();
				foreach (Pawn pawn in hashSet.Where(x => x.Spawned))
				{
					if (underCondition)
					{
						Dictionary<Thing, float> dangers = new Dictionary<Thing, float>();
						Thing closestDanger = null;
						float closestDangerDistance = float.MaxValue;
						bool hasLOSofAnyDanger = false;

						if (MapDic.ContainsKey(pawn.Map))
						{
							dangers = MapDic[pawn.Map];
						}
						else
						{
							foreach (Thing danger in GetListOfDangers(pawn))
							{
								float dangerDistance = danger.Position.DistanceTo(pawn.Position);
								dangers[danger] = dangerDistance;
								hasLOSofAnyDanger = hasLOSofAnyDanger ? hasLOSofAnyDanger : GenSight.LineOfSight(pawn.Position, danger.Position, pawn.Map);

								if (dangerDistance < closestDangerDistance)
								{
									closestDangerDistance = dangerDistance;
									closestDanger = danger;
								}
							}
						}

						if ((!pawn.Drafted || (alsoWhenDrafted && dangers.Any(entry => ((entry.Key.def.devNote != null && entry.Key.def.devNote == "insect") || (entry.Key.def.race != null && entry.Key.def.race.FleshType == FleshTypeDefOf.Insectoid)) && entry.Value < MinMaxFleeDistance.x && GenSight.LineOfSight(pawn.Position, entry.Key.Position, pawn.Map)))) && hasLOSofAnyDanger)
						{
							if (dangers != null && dangers.Count != 0)
							{
								if (closestDangerDistance < MinMaxFleeDistance.x)
								{
									BOTFleeParams param = new BOTFleeParams
									{
										Threat = closestDanger,
										Distance = MinMaxFleeDistance,
										Threats = dangers.Keys.ToList(),
										StayWhenNowhereToGo = false
									};

									pawn.MakeFlee(param);
									pawn.TryGainMemory(BOTThoughtDefOf.BOT_PyrophobicNearFire, 1);
								}
							}
						}
						else if (pawn.Drafted && hasLOSofAnyDanger)
						{
							if (dangers != null && dangers.Count != 0)
							{
								if (closestDangerDistance < MinMaxFleeDistance.x)
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
				foreach (KeyValuePair<Pawn, int> keyValuePair in MetabolismPawns.Where(x => x.Key.Spawned))
				{
					//Get 25% more/less healing by running the healing part of the HealthTick function
					Pawn p = keyValuePair.Key;
					if (!IsPawnStillThere(p)) return;
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
				foreach (KeyValuePair<Pawn, int> keyValuePair in NarcolepticPawns.Where(x => x.Key.Spawned))
				{
					if (!IsPawnStillThere(keyValuePair.Key)) return;
					float baseSleepChance = 0.015625f;
					float sleepChance = baseSleepChance;
					if (NarcolepticPawns[keyValuePair.Key] > 120000)
					{
						sleepChance *= 8f;
					}
					else if (NarcolepticPawns[keyValuePair.Key] > 60000)
					{
						sleepChance *= 4f;
					}
					else if (NarcolepticPawns[keyValuePair.Key] > 30000)
					{
						sleepChance *= 2f;
					}

					if (NarcolepticPawns[keyValuePair.Key] > 15000 && keyValuePair.Key.Spawned)
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
					NarcolepticPawns[p] = 0;
				}

				foreach (Pawn p in increment)
				{
					NarcolepticPawns[p] += 1000;
				}
			}
		}

		private void ManageSleepyHeads(int whenTicksDivisibleBy)
		{
			if (GameTicksDivisibleBy(whenTicksDivisibleBy))
			{
				List<Pawn> toRemove = new List<Pawn>();
				foreach(Pawn p in SleepyHeadPawns.Where(x => x.Spawned))
				{
					if (!IsPawnStillThere(p)) return;
					if (Rand.Value > 0.1 && p.CurJobDef == JobDefOf.LayDown)
					{
						p.TryGainMemory(BOTThoughtDefOf.BOT_SleepyHeadStopsSleeping, 0);
						p.jobs.EndCurrentJob(JobCondition.Succeeded);
						toRemove.Add(p);
					} 
					//Might got drafted or removed from the bed for other reasons
					else if (p.Drafted)
					{
						p.TryGainMemory(BOTThoughtDefOf.BOT_SleepyHeadForcefullyWokenUp, 0);
						toRemove.Add(p);
					}
					else if (p.CurJobDef != JobDefOf.LayDown)
                    {
						toRemove.Add(p);
					}
					else
					{
						p.TryGainMemory(BOTThoughtDefOf.BOT_SleepyHeadContinuesSleeping, 0);
					}
				}

				foreach(Pawn p in toRemove)
				{
					SleepyHeadPawns.Remove(p);
					p.needs.rest.CurLevelPercentage = 1f;
				}
			}
		}

		public static bool IsPawnStillThere(Pawn pawn)
        {
			bool flag = pawn != null && pawn.story != null && pawn.story.traits != null;

			if (!flag)
			{
				Log.Error("One of your pawns dissapeared in a way that was not accounted for in the mod: Bundle of Traits. Please message me the developer about this and if possible, tell me if you have an idea about what might have caused this.");

				if (pawn != null && pawn.Name != null)
				{
					Log.Error("Pawn name: " + pawn.Name + " Pawn is spawned: " + pawn.Spawned + " Pawn has story: " + (pawn.story != null) + " Pawn has traits: " + (pawn.story.traits != null));
				}
				else
				{
					Log.Error("For some reason, the Pawn doesn't exist anymore. Has name: " + (pawn.Name != null));
				}
			}
			return flag;
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
				AddPawnToHashSetIfPawnHasTrait(pawn, PyrophobicPawns, BOTTraitDefOf.BOT_Pyrophobia);
				AddPawnToHashSetIfPawnHasTrait(pawn, EntomophobicPawns, BOTTraitDefOf.BOT_Entomophobia);
				AddPawnToDicIfPawnHasTrait(pawn, MetabolismPawns, BOTTraitDefOf.BOT_Metabolism, 0);
				AddPawnToDicIfPawnHasTrait(pawn, NarcolepticPawns, BOTTraitDefOf.BOT_Narcoleptic, 0);
			}
		}

		private void AddPawnToHashSetIfPawnHasTrait(Pawn pawn, HashSet<Pawn> set, TraitDef traitDef)
		{
			if (pawn.story.traits.HasTrait(traitDef))
			{
				set.Add(pawn);
			}
		}

		private void AddPawnToDicIfPawnHasTrait<T>(Pawn pawn, Dictionary<Pawn, T> dic, TraitDef traitDef, T defaultValue)
		{
			if (pawn.story.traits.HasTrait(traitDef))
			{
				if (!dic.ContainsKey(pawn))
				{
					dic[pawn] = defaultValue;
				}
			}
		}

		/// <summary>
		///		Checks if the pawns in the dictionaries and hashlists should be inside them
		/// </summary>
		/// <param name="whenTicksDivisibleBy"></param>
		public void RemoveWrongPawnsFromSets(int whenTicksDivisibleBy)
		{
			if (GameTicksDivisibleBy(whenTicksDivisibleBy))
			{
				RemoveWrongPawnsFromDic(NarcolepticPawns, BOTTraitDefOf.BOT_Narcoleptic);
				RemoveWrongPawnsFromDic(Loves_SleepPawns, BOTTraitDefOf.BOT_Narcoleptic);
				RemoveWrongPawnsFromDic(MetabolismPawns, BOTTraitDefOf.BOT_Metabolism);

				PyrophobicPawns.RemoveWhere((Pawn p) => !p.story.traits.HasTrait(BOTTraitDefOf.BOT_Pyrophobia));
				EntomophobicPawns.RemoveWhere((Pawn p) => !p.story.traits.HasTrait(BOTTraitDefOf.BOT_Entomophobia));
			}
		}

		public void RemoveWrongPawnsFromDic<T>(Dictionary<Pawn, T> dic, TraitDef traitDef)
		{
			List<Pawn> removePawns = new List<Pawn>();
			foreach (KeyValuePair<Pawn, T> keyValuePair in dic)
			{
				if (!keyValuePair.Key.HasTrait(traitDef))
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
			NarcolepticPawns.Remove(pawn);
			Loves_SleepPawns.Remove(pawn);
			MetabolismPawns.Remove(pawn);
			Nyctophobes.Remove(pawn);
			SleepyHeadPawns.Remove(pawn);
			PyrophobicPawns.Remove(pawn);
		}

		public HashSet<Pawn> GetNyctophobes()
		{
			return Nyctophobes;
		}

		public Dictionary<Pawn, float> GetLoves_SleepDic()
		{
			return Loves_SleepPawns;
		}

		public HashSet<Pawn> GetSleepyHeadSet()
		{
			return SleepyHeadPawns;
		}

		private List<Thing> GetListOfDangers(Pawn pawn)
		{
			List<Thing> dangers = new List<Thing>();

			List<Thing> fireList = pawn.Map.listerThings.ThingsOfDef(ThingDefOf.Fire);
			if (pawn.story.traits.HasTrait(BOTTraitDefOf.BOT_Pyrophobia) && fireList.Count > 0)
			{
				dangers = fireList.ListFullCopy();
			}

			if (pawn.story.traits.HasTrait(BOTTraitDefOf.BOT_Entomophobia))
			{
				dangers.AddRange(pawn.Map.mapPawns.AllPawnsSpawned.Where(x => x.def.devNote == "insect" || x.def.race.FleshType == FleshTypeDefOf.Insectoid).Cast<IAttackTargetSearcher>().Cast<Thing>());
			}

			return dangers;
		}

		private float GetCandidateWeight(Pawn pawn, Pawn candidate)
		{
			float num = Mathf.Min(pawn.Position.DistanceTo(candidate.Position) / 40f, 1f);
			return 1f - num + 0.01f;
		}
	}

	[StaticConstructorOnStartup]
	class BOTInCodeDefEdits
	{
		static BOTInCodeDefEdits()
		{
			BOTTraitDefOf.BOT_Apathetic.DataAtDegree(0).disallowedInspirations = DefDatabase<InspirationDef>.AllDefsListForReading;
			BOTTraitDefOf.BOT_Apathetic.conflictingPassions = DefDatabase<SkillDef>.AllDefsListForReading;
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

