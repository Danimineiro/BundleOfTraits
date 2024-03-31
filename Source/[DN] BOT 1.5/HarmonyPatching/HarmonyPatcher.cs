using HarmonyLib;
using More_Traits.HarmonyPatching.Patches;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.HarmonyPatching
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatcher
    {
        static HarmonyPatcher()
        {
            Harmony harmony = new Harmony("BOT_Patcher");
            harmony.Patch(typeof(Toils_Ingest).GetMethod(nameof(Toils_Ingest.FinalizeIngest)), postfix: new HarmonyMethod(typeof(EclecticPalate_FinalizeIngestPatch), nameof(EclecticPalate_FinalizeIngestPatch.Postfix)));
            harmony.Patch(typeof(PawnDiedOrDownedThoughtsUtility).GetMethod("AppendThoughts_ForHumanlike"), postfix: new HarmonyMethod(typeof(Pacifist_WittnessDeath), nameof(Pacifist_WittnessDeath.Postfix)));
            harmony.Patch(typeof(PawnDiedOrDownedThoughtsUtility).GetMethod("AppendThoughts_Relations"), postfix: new HarmonyMethod(typeof(Pacifist_KilledAnimal), nameof(Pacifist_KilledAnimal.Postfix)));
            harmony.Patch(typeof(Toils_LayDown).GetMethod(nameof(Toils_LayDown.LayDown)), postfix: new HarmonyMethod(typeof(Nyctophobe_CanNotSleep), nameof(Nyctophobe_CanNotSleep.Postfix)));

            harmony.Patch(typeof(Pawn).GetMethod(nameof(Pawn.SpawnSetup)), postfix: new HarmonyMethod(typeof(HediffTraitPatches), nameof(HediffTraitPatches.SpawnSetup)));
            harmony.Patch(typeof(TraitSet).GetMethod(nameof(TraitSet.GainTrait)), postfix: new HarmonyMethod(typeof(HediffTraitPatches), nameof(HediffTraitPatches.GainTrait)));
        }
    }
}
