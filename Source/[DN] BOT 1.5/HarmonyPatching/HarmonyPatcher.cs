using HarmonyLib;
using More_Traits.HarmonyPatching.Patches;
using More_Traits.HarmonyPatching.Patches.Eclectic;
using More_Traits.HarmonyPatching.Patches.LovesSleeping;
using More_Traits.HarmonyPatching.Patches.Nyctophobe;
using More_Traits.HarmonyPatching.Patches.Pacifist;
using RimWorld;

using static HarmonyLib.AccessTools;
using Verse;
using More_Traits.HarmonyPatching.Patches.SleepyHead;

namespace More_Traits.HarmonyPatching
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatcher
    {
        static HarmonyPatcher()
        {
            Harmony harmony = new Harmony("BOT_Patcher");
            harmony.Patch(PropertyGetter(typeof(JobDriver_LayDown), nameof(JobDriver_LayDown.LookForOtherJobs)), postfix: new HarmonyMethod(typeof(SleepyHead_LayDown), nameof(SleepyHead_LayDown.LookForOtherJobs_Postfix)));
            harmony.Patch(Method(typeof(Toils_Ingest), nameof(Toils_Ingest.FinalizeIngest)), postfix: new HarmonyMethod(typeof(EclecticPalate_FinalizeIngestPatch), nameof(EclecticPalate_FinalizeIngestPatch.Postfix)));
            harmony.Patch(Method(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_ForHumanlike"), postfix: new HarmonyMethod(typeof(Pacifist_WittnessDeath), nameof(Pacifist_WittnessDeath.Postfix)));
            harmony.Patch(Method(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_Relations"), postfix: new HarmonyMethod(typeof(Pacifist_KilledAnimal), nameof(Pacifist_KilledAnimal.Postfix)));
            harmony.Patch(Method(typeof(JobDriver_LayDown), "MakeNewToils"), postfix: new HarmonyMethod(typeof(LayDownJobDriver), nameof(LayDownJobDriver.MakeNewToils_PostFix)));
            harmony.Patch(Method(typeof(TraitSet), nameof(TraitSet.GainTrait)), postfix: new HarmonyMethod(typeof(HediffTraitPatches), nameof(HediffTraitPatches.GainTrait)));
            harmony.Patch(PropertyGetter(typeof(Need), "IsFrozen"), postfix: new HarmonyMethod(typeof(Loves_Sleeping_IsFrozen), nameof(Loves_Sleeping_IsFrozen.Postfix)));
            harmony.Patch(Method(typeof(Pawn), nameof(Pawn.SpawnSetup)), postfix: new HarmonyMethod(typeof(HediffTraitPatches), nameof(HediffTraitPatches.SpawnSetup)));
        }
    }
}
