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

namespace More_Traits.HarmonyPatching.Patches
{
    public static class Loves_Sleeping_IsFrozen
    {
        public static void Postfix(Need __instance, Pawn ___pawn, ref bool __result)
        {
            if (!(__instance is Need_Joy)) return;
            if (!___pawn.HasTrait(BOT_TraitDefOf.BOT_Loves_Sleeping)) return;

            __result = false;
        }
    }
}
