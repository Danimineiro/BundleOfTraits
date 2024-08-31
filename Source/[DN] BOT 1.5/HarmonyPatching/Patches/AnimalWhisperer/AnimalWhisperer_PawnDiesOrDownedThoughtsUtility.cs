using More_Traits.DefOfs;
using More_Traits.Extensions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.HarmonyPatching.Patches.AnimalWhisperer
{
    internal static class AnimalWhisperer_PawnDiesOrDownedThoughtsUtility
    {
        public static void AppendThoughts_Relations(Pawn victim, DamageInfo? dinfo, PawnDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtToAddToAll> outAllColonistsThoughts)
        {
            if (thoughtsKind == PawnDiedOrDownedThoughtsKind.Banished && victim.RaceProps.Animal)
            {
                List<DirectPawnRelation> directRelations = victim.relations.DirectRelations;

                for (int i = 0; i < directRelations.Count; i++)
                {
                    DirectPawnRelation relation = directRelations[i];
                    Pawn other = relation.otherPawn;

                    if (other.needs?.mood == null) return;
                    if (!PawnUtility.ShouldGetThoughtAbout(other, victim)) return;
                    if (relation.def != PawnRelationDefOf.Bond) return;

                    other.TryGainMemory(BOT_ThoughtDefOf.BOT_AnimalWhisperer_BondedAnimalBanished);
                }

                //PawnDiedOrDownedThoughtsUtility.< AppendThoughts_Relations > g__GiveThoughtsForAnimalBond | 8_0(ThoughtDefOf.BondedAnimalBanished, ref CS$<> 8__locals1);
            }
        }
    }
}
