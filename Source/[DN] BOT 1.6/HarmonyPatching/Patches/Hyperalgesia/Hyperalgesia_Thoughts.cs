using More_Traits.DefOfs;
using More_Traits.Extensions;
using RimWorld;
using Verse;

namespace More_Traits.HarmonyPatching.Patches.Hyperalgesia
{
    public static class Hyperalgesia_Thoughts
    {
        public static void MoodMultiplier_Post(Pawn p, ThoughtWorker __instance, ref float __result)
        {
            if (__instance is not ThoughtWorker_Pain) return;
            if (!p.HasTrait(BOT_TraitDefOf.BOT_Hyperalgesia)) return;
            __result *= 2f;
        }
    }
}
