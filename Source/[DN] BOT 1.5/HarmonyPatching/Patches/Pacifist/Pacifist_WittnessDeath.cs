using RimWorld;
using System.Collections.Generic;
using Verse.AI;
using Verse;
using More_Traits.Extensions;
using More_Traits.DefOfs;

namespace More_Traits.HarmonyPatching.Patches.Pacifist
{
    public class Pacifist_WittnessDeath
    {
        public static void Postfix(Pawn victim, DamageInfo? dinfo, PawnDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtToAddToAll> outAllColonistsThoughts)
        {
            if (thoughtsKind != PawnDiedOrDownedThoughtsKind.Died) return;
            if (dinfo?.Def.execution == true) return;

            foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive)
            {
                if (!IsValidPawn(pawn, victim)) continue;
                outIndividualThoughts.Add(new IndividualThoughtToAdd(BOT_ThoughtDefOf.BOT_WittnessedDeathPacifist, pawn));

                if (pawn != dinfo?.Instigator as Pawn) continue;
                outIndividualThoughts.Add(new IndividualThoughtToAdd(BOT_ThoughtDefOf.BOT_Pacifist_KilledHuman, pawn));
            }
        }

        public static bool IsValidPawn(Pawn pawn, Pawn victim)
        {
            if (pawn == victim) return false;
            if (!pawn.HasTrait(BOT_TraitDefOf.BOT_Pacifist)) return false;
            if (!ThoughtUtility.Witnessed(pawn, victim)) return false;
            if (pawn.MentalStateDef == MentalStateDefOf.SocialFighting && ((MentalState_SocialFighting)pawn.MentalState).otherPawn == victim) return true;

            return true;
        }
    }
}
