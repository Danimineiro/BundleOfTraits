﻿using More_Traits.DefOfs;
using More_Traits.Extensions;
using RimWorld;
using Verse;

namespace More_Traits.HarmonyPatching.Patches.LovesSleeping
{
    public static class Loves_Sleeping_IsFrozen
    {
        public static void Postfix(Need __instance, Pawn ___pawn, ref bool __result)
        {
            if (__instance is not Need_Joy) return;
            if (!___pawn.HasTrait(BOT_TraitDefOf.BOT_Loves_Sleeping)) return;

            __result = false;
        }
    }
}
