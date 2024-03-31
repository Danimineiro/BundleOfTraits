﻿using More_Traits.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.HarmonyPatching.Patches
{
    public class HediffTraitPatches
    {
        public static void GainTrait(Pawn ___pawn)
        {
            ___pawn.AddTraitHediffs();
        }

        public static void SpawnSetup(Pawn __instance)
        {
            if (__instance.CanHandlePawn()) __instance.AddTraitHediffs();
        }
    }
}
