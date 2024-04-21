using More_Traits.DefOfs;
using More_Traits.Extensions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.HarmonyPatching.Patches.Sadist
{
    public static class Sadist_StanceTracker
    {
        public static void NotifyDamageTaken(Pawn_StanceTracker __instance, DamageInfo dinfo)
        {
            Pawn victim = __instance.pawn;

            if (dinfo.Def == DamageDefOf.SurgicalCut) return;
            if (dinfo.Def == DamageDefOf.ExecutionCut) return;
            if (!(dinfo.Instigator is Pawn instigator)) return;

            if (instigator.HasTrait(BOT_TraitDefOf.BOT_Sadist))
            {
                instigator.needs.mood?.thoughts.memories.TryGainMemory(BOT_ThoughtDefOf.BOT_SadistHurtHumanlike);
            }

            foreach (Pawn witness in victim.Map.mapPawns.AllPawnsSpawned.Where(thePawn => IsValidWitness(thePawn, instigator, victim)))
            {
                bool flag = WitnessedEither(witness, instigator, victim);

                if (flag) witness.needs.mood?.thoughts.memories.TryGainMemory(BOT_ThoughtDefOf.BOT_SadistWitnessedDamage);
            }
        }

        private static bool IsValidWitness(Pawn witness, Pawn instigator, Pawn victim)
        {
            if (witness.Dead) return false;
            if (witness == victim) return false;
            if (witness == instigator) return false;

            return witness.HasTrait(BOT_TraitDefOf.BOT_Sadist);
        }

        private static bool WitnessedEither(Pawn witness, Pawn instigator, Pawn victim)
        {
            bool witnessed = false;

            if (instigator != null)
            {
                witnessed |= ThoughtUtility.Witnessed(witness, instigator);
            }

            if (victim != null)
            {
                witnessed |= ThoughtUtility.Witnessed(witness, victim);
            }

            return witnessed;
        }
    }
}
