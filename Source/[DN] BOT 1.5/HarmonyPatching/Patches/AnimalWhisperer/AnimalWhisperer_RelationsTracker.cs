using HarmonyLib;
using More_Traits.DefOfs;
using More_Traits.Extensions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.HarmonyPatching.Patches.AnimalWhisperer;

internal static class AnimalWhisperer_RelationsTracker
{
    public static void RemoveDirectRelation(PawnRelationDef def, Pawn_RelationsTracker __instance)
    {
        if (def != PawnRelationDefOf.Bond) return;

        Traverse.Create<Pawn_RelationsTracker>().Field<Pawn>("pawn").Value.TryGainMemory(BOT_ThoughtDefOf.BOT_AnimalWhisperer_SoldMyBondedAnimalMood);
    }
}
