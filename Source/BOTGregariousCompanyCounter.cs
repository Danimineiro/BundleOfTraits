using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace More_Traits
{
	class BOTGregariousCompanyCounter
	{
		private static List<Pawn> allAliveOrDead;
		public static Dictionary<Pawn, int> gregariousCompany = new Dictionary<Pawn, int>();

		[DebugAction("Mods", null, false, false, allowedGameStates = AllowedGameStates.Playing)]
		public static void BOTCalculateCompanyForGregariousPawn()
		{
			allAliveOrDead = PawnsFinder.All_AliveOrDead.ListFullCopy();

			if (allAliveOrDead.Count == 0) return;

			foreach (Pawn pawn in allAliveOrDead)
			{
				//If the player faction is not generated yet (null) and the generated pawn has a null faction, this would pass through without the extra null check
				if (pawn.def.race.Humanlike && pawn.HasTrait(BOTTraitDefOf.BOT_Gregarious) && !pawn.Dead && pawn.Faction != null && pawn.Faction == Faction.OfPlayerSilentFail)
				{
					//Create a list of company if not present
					if (!gregariousCompany.ContainsKey(pawn))
					{
						gregariousCompany[pawn] = 0;
					}

					if (pawn.Map != null && !pawn.IsPrisoner)
					{
						gregariousCompany[pawn] = pawn.Map.mapPawns.AllPawnsSpawned.FindAll(x => x.Faction == Faction.OfPlayerSilentFail && !x.IsPrisoner).Count - 1;
					} 
					else if (pawn.IsCaravanMember())
					{
						gregariousCompany[pawn] = pawn.GetCaravan().pawns.Count;
					}
				}
			}
		}
		public static int GetCountFor(Pawn pawn)
		{
			if (gregariousCompany.ContainsKey(pawn))
			{
				return gregariousCompany[pawn];
			} else BOTCalculateCompanyForGregariousPawn();

			return gregariousCompany[pawn];
		}
	}
}
