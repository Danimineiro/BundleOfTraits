using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using Verse.AI;

namespace More_Traits
{
	class BOTTraitsManager : GameComponent
	{
		public static string src = "I made a lot of code in this looking at Vanilla Traits Expanded. Check out their mod at: https://steamcommunity.com/sharedfiles/filedetails/?id=2296404655";
		private static IntVec2 PyrophobeMinMaxFleeDistance = new IntVec2(12,24);

		public static Dictionary<Pawn, int> Narcoleptics;
		public static HashSet<Pawn> Pyrophobics;

		private List<Pawn> NarcolepticPawnKeys = new List<Pawn>();
		private List<int> NarcolepticPawnInts = new List<int>();

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
									pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(BOTDefOf.BOT_PyrophobicNearFire, 1));
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
									pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(BOTDefOf.BOT_PyrophobicNearFire, 0));
								}
							}
						}
					}
				}
			}

			if (GameTicksDivisibleBy(1000))
			{
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

		public void AddPawn(Pawn pawn)
		{
			if (pawn.story != null && pawn.story.traits != null)
			{
				if (pawn.story.traits.HasTrait(BOTDefOf.BOT_Narcoleptic))
				{
					PreInit();
					Narcoleptics[pawn] = 0;
				}

				if (pawn.story.traits.HasTrait(BOTDefOf.BOT_Pyrophobia))
				{
					PreInit();
					Pyrophobics.Add(pawn);
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look<Pawn, int>(ref Narcoleptics, "Narcoleptics", LookMode.Reference, LookMode.Value, ref NarcolepticPawnKeys, ref NarcolepticPawnInts);
			Scribe_Collections.Look<Pawn>(ref Pyrophobics, "Pyrophobics", LookMode.Reference);
		}

		public void RemoveWrongPawnsFromSets()
		{
			List<Pawn> removeNarcoleptic = new List<Pawn>();
			foreach (KeyValuePair<Pawn, int> keyValuePair in Narcoleptics)
			{
				if (!keyValuePair.Key.story.traits.HasTrait(BOTDefOf.BOT_Narcoleptic))
				{
					removeNarcoleptic.Add(keyValuePair.Key);
				}
			}

			foreach(Pawn p in removeNarcoleptic)
			{
				Narcoleptics.Remove(p);
			}

			Pyrophobics.RemoveWhere((Pawn p) => !p.story.traits.HasTrait(BOTDefOf.BOT_Pyrophobia));
		}

		public void RemoveDestroyedPawnFromSets(Pawn pawn)
		{
			Narcoleptics.Remove(pawn);
			Pyrophobics.RemoveWhere((Pawn p) => p == pawn);
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
	public static class GainTrait_Patch
	{
		public static void Postfix(Pawn ___pawn)
		{
			Current.Game.GetComponent<BOTTraitsManager>().AddPawn(___pawn);
		}
	}

	[HarmonyPatch(typeof(Pawn), "SpawnSetup")]
	public static class SpawnSetup_Patch
	{
		public static void Postfix(Pawn __instance)
		{
			Current.Game.GetComponent<BOTTraitsManager>().AddPawn(__instance);
		}
	}

	[HarmonyPatch(typeof(Pawn), "Destroy")]
	public static class Destroy_Patch
	{
		public static void Prefix(Pawn __instance)
		{
			Current.Game.GetComponent<BOTTraitsManager>().RemoveDestroyedPawnFromSets(__instance);
		}
	}
}

