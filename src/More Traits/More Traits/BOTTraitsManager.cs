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

		public static HashSet<Pawn> Narcoleptics;
		public static HashSet<Pawn> Pyrophobics;

		static BOTTraitsManager()
		{
		}

		public BOTTraitsManager(Game game)
		{
		}

		public void PreInit()
		{
			if (Narcoleptics == null) Narcoleptics = new HashSet<Pawn>();
			if (Pyrophobics == null) Pyrophobics = new HashSet<Pawn>();

			RemoveWrongPawnsFromSets();
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
				foreach (Pawn pawn in Narcoleptics)
				{
				}

				Dictionary<Map, List<Thing>> MapFireDic = new Dictionary<Map, List<Thing>>();
				foreach (Pawn pawn in Pyrophobics)
				{
					if (!pawn.Drafted && pawn.Map != null && pawn.Map.fireWatcher.FireDanger > 0)
					{
						List<Thing> fires = null;

						if (MapFireDic.ContainsKey(pawn.Map))
						{
							fires = MapFireDic[pawn.Map];
						} else {
							fires = pawn.Map.listerThings.ThingsOfDef(ThingDefOf.Fire);
						}

						if (fires != null && fires.Count != 0)
						{
							float closestFireDistance = fires.Min(fire => fire.Position.DistanceTo(pawn.Position));
							if (closestFireDistance < PyrophobeMinMaxFleeDistance.x)
							{
								Utils.MakeFlee(pawn, fires.RandomElement(), PyrophobeMinMaxFleeDistance, fires, true);
							}
						}
					}
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
					Narcoleptics.Add(pawn);
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
			Scribe_Collections.Look<Pawn>(ref Narcoleptics, "Narcoleptics", LookMode.Reference);
			Scribe_Collections.Look<Pawn>(ref Pyrophobics, "Pyrophobics", LookMode.Reference);
		}

		public void RemoveWrongPawnsFromSets()
		{
			Narcoleptics.RemoveWhere((Pawn p) => !p.story.traits.HasTrait(BOTDefOf.BOT_Narcoleptic));
			Pyrophobics.RemoveWhere((Pawn p) => !p.story.traits.HasTrait(BOTDefOf.BOT_Pyrophobia));
		}

		public void RemoveDestroyedPawnFromSets(Pawn pawn)
		{
			Narcoleptics.RemoveWhere((Pawn p) => p == pawn);
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

	public static class Utils
	{
		public static void MakeFlee(Pawn pawn, Thing threat, int distance, List<Thing> threats, bool stayWhenNowhereToGo = false)
		{
			MakeFlee(pawn, threat, new IntVec2(distance, distance), threats, stayWhenNowhereToGo);
		}

		public static void MakeFlee(Pawn pawn, Thing threat, int minDistance, int maxDistance, List<Thing> threats, bool stayWhenNowhereToGo = false)
		{
			MakeFlee(pawn, threat, new IntVec2(minDistance, maxDistance), threats, stayWhenNowhereToGo);
		}

		public static void MakeFlee(Pawn pawn, Thing threat, IntVec2 distance, List<Thing> threats, bool stayWhenNowhereToGo = false)
		{
			if (distance.x > distance.z)
			{
				Log.Warning("MakeFlee was called where minDistance was bigger than maxDistance. Min: " + distance.x + " Max: " + distance.z + "!");
				distance.z = distance.x;
			}

			Job job = null;
			IntVec3 intVec3;
			if (pawn.CurJob != null && pawn.CurJob.def == JobDefOf.Flee)
			{
				//Continue Fleeing
				intVec3 = pawn.CurJob.targetA.Cell;
			}
			else
			{
				//Find a place to flee to
				intVec3 = CellFinderLoose.GetFleeDest(pawn, threats, Rand.Range(distance.x, distance.z));
			}
			if (intVec3 == pawn.Position && !stayWhenNowhereToGo)
			{
				//Find a random place to flee to because there was nowhere to go
				intVec3 = GenRadial.RadialCellsAround(pawn.Position, (float)distance.x, (float)(distance.z)).RandomElement();
			}
			if (intVec3 != pawn.Position)
			{
				job = JobMaker.MakeJob(JobDefOf.Flee, intVec3, threat);
			}
			if (job != null)
			{
				pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, false);
			}
		}
	}
}

