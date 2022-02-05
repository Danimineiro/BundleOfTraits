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
			Scribe_Collections.Look(ref NarcolepticPawns, "Narcoleptics", LookMode.Reference, LookMode.Value, ref NarcolepticPawnKeys, ref NarcolepticPawnInts);
			Scribe_Collections.Look(ref Loves_SleepPawns, "Loves_Sleep", LookMode.Reference, LookMode.Value, ref Loves_SleepPawnKeys, ref Loves_SleepInitialRestPercentage);
			Scribe_Collections.Look(ref PyrophobicPawns, "Pyrophobics", LookMode.Reference);
			Scribe_Collections.Look(ref SleepyHeadPawns, "SleepyHeads", LookMode.Reference);
			Scribe_Collections.Look(ref EntomophobicPawns, "Entomophobics", LookMode.Reference);
			Scribe_Collections.Look(ref MetabolismPawns, "MetabolismPawns", LookMode.Reference, LookMode.Value, ref MetabolismPawnKeys, ref MetabolismPawnInts);
			Scribe_Collections.Look(ref Nyctophobes, "Nyctophobes", LookMode.Reference);
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

			ManageFleeing(fleeIntervallInTicks, PyrophobicPawns);
			ManageFleeing(fleeIntervallInTicks, EntomophobicPawns);
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
        private bool GameTicksDivisibleBy(int n) => Find.TickManager.TicksGame % n == 0;

        private bool PawnScaredOfInsectsAndInsectsNearby(Dictionary<Thing, float> dangers, Pawn pawn) => dangers.Any(entry => ((entry.Key.def.devNote != null && entry.Key.def.devNote == "insect") || (entry.Key.def.race != null && entry.Key.def.race.FleshType == FleshTypeDefOf.Insectoid)) && entry.Value < MinMaxFleeDistance.x && GenSight.LineOfSight(pawn.Position, entry.Key.Position, pawn.Map));

		private void ProcessDraftedPawnMemories(Pawn pawn, float closestDangerDistance)
		{
			if (closestDangerDistance < MinMaxFleeDistance.x)
			{
				pawn.TryGainMemory(BOTThoughtDefOf.BOT_PyrophobicNearFire, 0);
			}
		}

		private void ProcessFleeingPawns(Pawn pawn, Thing closestDanger, float closestDangerDistance, Dictionary<Thing, float> dangers)
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

		private bool ShouldSkipFleeingForPawn(Pawn pawn)
        {
			if (pawn?.Map == null || !pawn.Spawned) return true;

			if (PyrophobicPawns.Contains(pawn) && (pawn.Map.fireWatcher?.FireDanger ?? 0f) > 0f)
            {
				return false;
            }

			if (EntomophobicPawns.Contains(pawn) && (pawn.Map.mapPawns?.AllPawnsSpawned.Exists(x => x?.def?.devNote == "insect" || x?.def?.race?.FleshType == FleshTypeDefOf.Insectoid) ?? false))
            {
				return false;
            }

			return true;
        }

		private Dictionary<Thing, float> CreateDangersDicFor(Pawn pawn, List<Thing> dangersList, out Thing closestDanger, out float closestDangerDistance)
        {
			Dictionary<Thing, float> dangers = new Dictionary<Thing, float>();
			closestDanger = new Thing();
			closestDangerDistance = int.MaxValue;

			foreach (Thing danger in dangersList)
			{
				float dangerDistance = danger.Position.DistanceTo(pawn.Position);
				dangers[danger] = dangerDistance;

				if (dangerDistance < closestDangerDistance)
				{
					closestDangerDistance = dangerDistance;
					closestDanger = danger;
				}
			}

			return dangers;
		}

		private void ManageFleeing(int whenTicksDivisibleBy, HashSet<Pawn> hashSet)
		{
			if (!GameTicksDivisibleBy(whenTicksDivisibleBy)) return;

			Dictionary<Map, Dictionary<Thing, float>> MapDic = new Dictionary<Map, Dictionary<Thing, float>>();
			foreach (Pawn pawn in hashSet.Where(pawn => !ShouldSkipFleeingForPawn(pawn)))
			{
				Dictionary<Thing, float> dangers = new Dictionary<Thing, float>();
                bool flag = MapDic.ContainsKey(pawn.Map);

				if (flag) dangers = MapDic[pawn.Map];

				dangers = CreateDangersDicFor(pawn, flag ? new List<Thing>(dangers.Keys) : GetListOfDangers(pawn), out Thing closestDanger, out float closestDangerDistance);

				if ((!pawn.Drafted || PawnScaredOfInsectsAndInsectsNearby(dangers, pawn)) && pawn.HasSightOfAnyIn(dangers.Keys.ToList()))
				{
					ProcessFleeingPawns(pawn, closestDanger, closestDangerDistance, dangers);
				}
				else if (pawn.Drafted && pawn.HasSightOfAnyIn(dangers.Keys.ToList()))
				{
					ProcessDraftedPawnMemories(pawn, closestDangerDistance);
				}
			}
		}

		private float HealSpeedBaseFor(Pawn pawn)
        {
			float num = 8f;

			if (pawn.GetPosture() != PawnPosture.Standing)
			{
				num += 4f;
				Building_Bed building_Bed = pawn.CurrentBed();
				if (building_Bed != null)
				{
					num += building_Bed.def.building.bed_healPerDay;
				}
			}

			return num;
		}

		private float HealSpeedBaseAfterHediffsFor(Pawn pawn)
        {
			float num = HealSpeedBaseFor(pawn);

			HediffSet hediffSet = pawn.health.hediffSet;
			foreach (Hediff hediff in hediffSet.hediffs)
			{
				HediffStage curStage = hediff.CurStage;
				if (curStage != null && curStage.naturalHealingFactor != -1f)
				{
					num *= curStage.naturalHealingFactor;
				}
			}

			return num;
		}

		private void MetabolismHealOrHurtNaturallyHealingInjury(Pawn pawn, out bool processedAny)
        {
			processedAny = false;
			HediffSet hediffSet = pawn.health.hediffSet;
			if (!hediffSet.HasNaturallyHealingInjury()) return;

			float amount = HealSpeedBaseAfterHediffsFor(pawn) * pawn.HealthScale * 0.01f * pawn.GetStatValue(StatDefOf.InjuryHealingFactor, true);

			if (pawn.story.traits.GetTrait(BOTTraitDefOf.BOT_Metabolism).Degree == -1)
			{
				(from x in hediffSet.GetHediffs<Hediff_Injury>()
				 where x.CanHealNaturally()
				 select x).RandomElement().Heal(amount);
				processedAny = true;
			}
			else
			{
				(from x in hediffSet.GetHediffs<Hediff_Injury>()
				 where x.CanHealNaturally()
				 select x).RandomElement().Injure(amount);
			}
		}

		private void MetabolismHealOrHurtTendedAndHealingInjury(Pawn pawn, out bool processedAny)
		{
			processedAny = false;
			HediffSet hediffSet = pawn.health.hediffSet;
			if (hediffSet.HasTendedAndHealingInjury() && !pawn.Starving())
			{
				Hediff_Injury hediff_Injury = (from x in hediffSet.GetHediffs<Hediff_Injury>()
											   where x.CanHealFromTending()
											   select x).RandomElement();

				float tendQuality = hediff_Injury.TryGetComp<HediffComp_TendDuration>().tendQuality;
				float amount = 8f * GenMath.LerpDouble(0f, 1f, 0.5f, 1.5f, Mathf.Clamp01(tendQuality)) * pawn.HealthScale * 0.01f * pawn.GetStatValue(StatDefOf.InjuryHealingFactor, true);

				if (pawn.story.traits.GetTrait(BOTTraitDefOf.BOT_Metabolism).Degree == -1)
				{
					hediff_Injury.Heal(amount);
					processedAny = true;
				}
				else
				{
					hediff_Injury.Injure(amount);
				}
			}
		}

		private void MetabolismPawnExtraHealTick(Pawn pawn, out bool processedAny)
		{
			MetabolismHealOrHurtNaturallyHealingInjury(pawn, out bool temp);
			MetabolismHealOrHurtTendedAndHealingInjury(pawn, out bool temp2);
			processedAny = temp || temp2;
		}

		private void ManageMetabolism(int whenTicksDivisibleBy)
		{
			if (!GameTicksDivisibleBy(whenTicksDivisibleBy)) return;

			foreach (KeyValuePair<Pawn, int> keyValuePair in MetabolismPawns.Where(pair => pair.Key.Spawned))
			{
				//Get 25% more/less healing by running the healing part of the HealthTick function
				Pawn pawn = keyValuePair.Key;
				if (!IsPawnStillThere(pawn) || pawn.RaceProps.IsFlesh && !pawn.Starving()) return;
				HediffSet hediffSet = pawn.health.hediffSet;

				//Heal fast Metabolism, damage flow by the same amount it was healed.
				MetabolismPawnExtraHealTick(pawn, out bool processedAny);

				if (processedAny && pawn.IsFullyHealed() && PawnUtility.ShouldSendNotificationAbout(pawn))
				{
					Messages.Message("MessageFullyHealed".Translate(pawn.LabelCap, pawn), pawn, MessageTypeDefOf.PositiveEvent, true);
				}
			}
		}

		private float NarcolepticSleepChance(Pawn pawn)
		{
			float baseSleepChance = 0.015625f;
			float sleepChance = baseSleepChance;
			if (NarcolepticPawns[pawn] > 120000)
			{
				sleepChance *= 8f;
			}
			else if (NarcolepticPawns[pawn] > 60000)
			{
				sleepChance *= 4f;
			}
			else if (NarcolepticPawns[pawn] > 30000)
			{
				sleepChance *= 2f;
			}

			return sleepChance;
		}

		private bool NarcolepticShouldFallAsleep(Pawn pawn)
        {
			return NarcolepticPawns[pawn] > 15000 && pawn.Spawned && Rand.Value < NarcolepticSleepChance(pawn) && (pawn.CurJob == null || pawn.CurJob.def != JobDefOf.LayDown);
		}

		private void MakeNarcolepticFallAsleep(Pawn pawn)
		{
			pawn.jobs.StartJob(JobMaker.MakeJob(JobDefOf.LayDown, pawn.Position), JobCondition.InterruptForced, null, false, true, null, new JobTag?(JobTag.SatisfyingNeeds), false, false);
			if (pawn.InMentalState && pawn.MentalStateDef.recoverFromCollapsingExhausted)
			{
				pawn.mindState.mentalStateHandler.CurState.RecoverFromState();
			}
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				Messages.Message("BOTNarcolepticInvoluntarySleep".Translate(pawn.LabelShort, pawn), pawn, MessageTypeDefOf.NegativeEvent, true);
			}
		}

		private void ManageNarcoleptics(int whenTicksDivisibleBy)
		{
			if (!GameTicksDivisibleBy(whenTicksDivisibleBy)) return;

			//Narcoleptics
			List<Pawn> reset = new List<Pawn>();
			List<Pawn> increment = new List<Pawn>();
			foreach (KeyValuePair<Pawn, int> keyValuePair in NarcolepticPawns.Where(x => x.Key.Spawned))
			{
				if (!IsPawnStillThere(keyValuePair.Key)) return;

				if (NarcolepticShouldFallAsleep(keyValuePair.Key))
				{
					MakeNarcolepticFallAsleep(keyValuePair.Key);
					reset.Add(keyValuePair.Key);
				}
				else if (!keyValuePair.Key.IsAsleep())
				{
					increment.Add(keyValuePair.Key);
				}
			}

			reset.ForEach(pawn => NarcolepticPawns[pawn] = 0);
			increment.ForEach(pawn => NarcolepticPawns[pawn] += 1000);
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
            if (pawn.story == null || pawn.story.traits == null) return;

            AddPawnToHashSetIfPawnHasTrait(pawn, PyrophobicPawns, BOTTraitDefOf.BOT_Pyrophobia);
            AddPawnToHashSetIfPawnHasTrait(pawn, EntomophobicPawns, BOTTraitDefOf.BOT_Entomophobia);
            AddPawnToDicIfPawnHasTrait(pawn, MetabolismPawns, BOTTraitDefOf.BOT_Metabolism, 0);
            AddPawnToDicIfPawnHasTrait(pawn, NarcolepticPawns, BOTTraitDefOf.BOT_Narcoleptic, 0);

            AddCongenitalAnalgesiaTrait(pawn);
        }

        private static void AddCongenitalAnalgesiaTrait(Pawn pawn)
        {
            if (pawn.HasTrait(BOTTraitDefOf.BOT_CongenitalAnalgesia) && !pawn.health.hediffSet.HasHediff(BOTHediffDefOf.BOT_CongenitalAnalgesiaPainReducer))
            {
                pawn.health.AddHediff(BOTHediffDefOf.BOT_CongenitalAnalgesiaPainReducer);
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

				PyrophobicPawns.RemoveWhere((Pawn p) => !p.HasTrait(BOTTraitDefOf.BOT_Pyrophobia));
				EntomophobicPawns.RemoveWhere((Pawn p) => !p.HasTrait(BOTTraitDefOf.BOT_Entomophobia));
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

