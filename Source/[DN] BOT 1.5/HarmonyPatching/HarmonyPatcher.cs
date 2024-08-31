using static HarmonyLib.AccessTools;
using HarmonyLib;
using RimWorld;
using Verse;
using System;
using More_Traits.HarmonyPatching.Patches.Apathetic;
using More_Traits.HarmonyPatching.Patches.Sadist;
using More_Traits.HarmonyPatching.Patches.Communal;
using More_Traits.HarmonyPatching.Patches.Vulgar;
using More_Traits.HarmonyPatching.Patches;
using More_Traits.HarmonyPatching.Patches.Eclectic;
using More_Traits.HarmonyPatching.Patches.LovesSleeping;
using More_Traits.HarmonyPatching.Patches.Pacifist;
using More_Traits.HarmonyPatching.Patches.Hyperalgesia;
using More_Traits.HarmonyPatching.Patches.Chinophile;
using More_Traits.HarmonyPatching.ModCompatibility;
using Verse.AI;
using More_Traits.HarmonyPatching.Patches.AnimalWhisperer;

namespace More_Traits.HarmonyPatching
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatcher
    {
        public static float SaturatedXPMult => madSkillsSaturatedXPMult();

        private static Func<float> madSkillsSaturatedXPMult = () => 0.2f;

        static HarmonyPatcher()
        {
            Harmony harmony = new("BOT_Patcher");

            harmony.Patch(Method(typeof(SkillRecord), nameof(SkillRecord.LearnRateFactor)), prefix: new(typeof(Apathetic_LearnRateFactor), nameof(Apathetic_LearnRateFactor.LearnRateFactor)));
            harmony.Patch(Method(typeof(SkillUI), nameof(SkillUI.GetLearningFactor)), prefix: new(typeof(Apathetic_SkillUI), nameof(Apathetic_SkillUI.GetLearningFactor_Prefix)));
            harmony.Patch(Method(typeof(SkillUI), "GetSkillDescription"), prefix: new(typeof(Apathetic_SkillUI), nameof(Apathetic_SkillUI.GetSkillDescription_Marker)));

            harmony.Patch(Method(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_ForHumanlike"), postfix: new (typeof(Pacifist_WittnessDeath), nameof(Pacifist_WittnessDeath.Postfix)));
            harmony.Patch(Method(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_Relations"), postfix: new(typeof(Pacifist_KilledAnimal), nameof(Pacifist_KilledAnimal.Postfix)));
            harmony.Patch(Method(typeof(Pawn_InteractionsTracker), "CheckSocialFightStart"), postfix: new(typeof(Vulgar_SocialFightStart), nameof(Vulgar_SocialFightStart.Postfix)));
            harmony.Patch(Method(typeof(ThoughtUtility), "RemovePositiveBedroomThoughts"), postfix: new(typeof(Communal_RemoveThoughts), nameof(Communal_RemoveThoughts.Postfix)));
            harmony.Patch(Method(typeof(JobDriver_LayDown), "MakeNewToils"), postfix: new(typeof(LayDownJobDriver), nameof(LayDownJobDriver.MakeNewToils_PostFix)));
            harmony.Patch(PropertyGetter(typeof(Need), "IsFrozen"), postfix: new(typeof(Loves_Sleeping_IsFrozen), nameof(Loves_Sleeping_IsFrozen.Postfix)));

            harmony.Patch(Method(typeof(InteractionWorker_Insult), nameof(InteractionWorker_Insult.RandomSelectionWeight)), postfix: new(typeof(Vulgar_InsultWorker), nameof(Vulgar_InsultWorker.RandomSelectionWeight)));
            harmony.Patch(Method(typeof(Recipe_RemoveBodyPart), nameof(Recipe_RemoveBodyPart.ApplyThoughts)), postfix: new(typeof(Sadist_RemoveBodyPart), nameof(Sadist_RemoveBodyPart.ApplyThoughtsPatch)));
            harmony.Patch(Method(typeof(Toils_Ingest), nameof(Toils_Ingest.FinalizeIngest)), postfix: new(typeof(EclecticPalate_FinalizeIngestPatch), nameof(EclecticPalate_FinalizeIngestPatch.Postfix)));
            harmony.Patch(Method(typeof(Pawn_StanceTracker), nameof(Pawn_StanceTracker.Notify_DamageTaken)), postfix: new(typeof(Sadist_StanceTracker), nameof(Sadist_StanceTracker.NotifyDamageTaken)));
            harmony.Patch(Method(typeof(ThoughtWorker), nameof(ThoughtWorker.MoodMultiplier)), postfix: new(typeof(Hyperalgesia_Thoughts), nameof(Hyperalgesia_Thoughts.MoodMultiplier_Post)));
            harmony.Patch(Method(typeof(TraitSet), nameof(TraitSet.GainTrait)), postfix: new(typeof(HediffTraitPatches), nameof(HediffTraitPatches.GainTrait)));
            harmony.Patch(Method(typeof(Pawn), nameof(Pawn.SpawnSetup)), postfix: new(typeof(HediffTraitPatches), nameof(HediffTraitPatches.SpawnSetup)));
            harmony.Patch(Method(typeof(Pawn), "TicksPerMove"), postfix: new(typeof(Chinophile_Pawn), nameof(Chinophile_Pawn.TicksPerMove)));

            //Round 4
            harmony.Patch(Method(typeof(Pawn_MindState), "CheckStartMentalStateBecauseRecruitAttempted"), prefix: new(AnimalWhisperer_TamingFail.CheckStartMentalStateBecauseRecruitAttempted));
            harmony.Patch(Method(typeof(InteractionWorker_Nuzzle), nameof(InteractionWorker_Nuzzle.Interacted)), postfix: new(AnimalWhisperer_Nuzzled.InteractionWorker_Nuzzle_Interacted));
            harmony.Patch(Method(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.RemoveDirectRelation), [typeof(PawnRelationDef), typeof(Pawn)]), postfix: new(AnimalWhisperer_RelationsTracker.RemoveDirectRelation));

            VSEPatches(harmony);
            MadSkillsPatches(harmony);
        }

        private static void MadSkillsPatches(Harmony _)
        {
            if (!ModLister.HasActiveModWithName("Mad Skills")) return;
            madSkillsSaturatedXPMult = (Func<float>)PropertyGetter(TypeByName("RTMadSkills.ModSettings"), "saturatedXPMultiplier").CreateDelegate(typeof(Func<float>));
        }

        private static void VSEPatches(Harmony harmony)
        {
            if (!ModLister.HasActiveModWithName("Vanilla Skills Expanded")) return;
            harmony.Patch(Method(TypeByName("VSE.Passions.PassionPatches"), "GenerateSkills_Prefix"), prefix: new HarmonyMethod(typeof(VSE), nameof(VSE.Skip_GenerateSkill_PrefixPatch)));
            harmony.Patch(Method(TypeByName("VSE.Passions.LearnRateFactorCache"), "LearnRateFactorBase"), prefix: new HarmonyMethod(typeof(VSE), nameof(VSE.Skip_LearnRateFactorBase)));
        }
    }
}
