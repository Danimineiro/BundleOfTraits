using More_Traits.DefOfs;
using More_Traits.Extensions;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace More_Traits.HarmonyPatching.Patches.Pacifist
{
    public class Pacifist_KilledAnimal
    {
        public static void Postfix(Pawn victim, DamageInfo? dinfo, PawnDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts)
        {
            if (dinfo is not DamageInfo damageInfo) return;
            if (thoughtsKind != PawnDiedOrDownedThoughtsKind.Died) return;
            if (damageInfo.Instigator is not Pawn instigator) return;
            if (instigator == victim) return;
            if (victim.RaceProps.Animal) return;
            if (!instigator.HasTrait(BOT_TraitDefOf.BOT_Pacifist)) return;

            outIndividualThoughts.Add(new IndividualThoughtToAdd(BOT_ThoughtDefOf.BOT_Pacifist_KilledAnimal, instigator, null));
        }
    }
}
