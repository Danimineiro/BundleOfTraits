using More_Traits.DefOfs;
using More_Traits.Extensions;
using RimWorld;
using System.Linq;
using Verse;

namespace More_Traits.HarmonyPatching.Patches.Sadist
{
    public static class Sadist_StanceTracker
    {
        public static void NotifyDamageTaken(Pawn_StanceTracker __instance, DamageInfo dinfo)
        {
            Pawn victim = __instance.pawn;
            Pawn? instigator = dinfo.Instigator as Pawn;

            if (dinfo.Def == DamageDefOf.SurgicalCut) return;
            if (dinfo.Def == DamageDefOf.ExecutionCut) return;

            if (instigator?.HasTrait(BOT_TraitDefOf.BOT_Sadist) == true)
            {
                instigator.needs.mood?.thoughts.memories.TryGainMemory(BOT_ThoughtDefOf.BOT_SadistHurtHumanlike);
            }

            foreach (Pawn witness in victim.Map.mapPawns.AllPawnsSpawned.Where(thePawn => IsValidWitness(thePawn, instigator, victim)))
            {
                witness.needs.mood?.thoughts.memories.TryGainMemory(BOT_ThoughtDefOf.BOT_SadistWitnessedDamage);
            }
        }

        private static bool IsValidWitness(Pawn witness, Pawn? instigator, Pawn victim)
        {
            if (witness.Dead) return false;
            if (witness == victim) return false;
            if (witness == instigator) return false;
            if (!witness.HasTrait(BOT_TraitDefOf.BOT_Sadist)) return false;

            return victim != null && ThoughtUtility.Witnessed(witness, victim);
        }
    }
}
