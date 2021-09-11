using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;

namespace More_Traits
{
	class BOTTraitsManager : GameComponent
	{
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
					Log.Message("Narco: " + pawn.Name);
                }
				foreach (Pawn pawn in Pyrophobics)
				{
					Log.Message("Pyro: " + pawn.Name);
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
}

