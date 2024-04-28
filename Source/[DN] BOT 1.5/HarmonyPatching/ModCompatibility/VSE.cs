using More_Traits.DefOfs;
using More_Traits.Extensions;
using RimWorld;
using Verse;

namespace More_Traits.HarmonyPatching.ModCompatibility
{
    public static class VSE
    {
        public static bool Skip_GenerateSkill_PrefixPatch(Pawn pawn, ref bool __result)
        {
            return !(__result = pawn.HasTrait(BOT_TraitDefOf.BOT_Apathetic));
        }

        public static bool Skip_LearnRateFactorBase(SkillRecord sr, ref float __result)
        {
            if (sr.Pawn.HasTrait(BOT_TraitDefOf.BOT_Apathetic))
            {
                __result = 1f;
                return false;
            }

            return true;
        }
    }
}
