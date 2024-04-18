using More_Traits.DefOfs;
using More_Traits.Extensions;
using Verse;

namespace More_Traits.HarmonyPatching.ModCompatibility
{
    public static class VSE
    {
        public static bool SkipGenerateSkill_PrefixPatch(Pawn pawn, ref bool __result)
        {
            __result = pawn.HasTrait(BOT_TraitDefOf.BOT_Apathetic);
            return !__result;
        }
    }
}
