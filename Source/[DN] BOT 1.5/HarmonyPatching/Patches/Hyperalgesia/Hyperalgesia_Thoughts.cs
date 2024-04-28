using More_Traits.DefOfs;
using More_Traits.Extensions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.HarmonyPatching.Patches.Hyperalgesia
{
    public static class Hyperalgesia_Thoughts
    {
        public static void MoodMultiplier_Post(Pawn p, ThoughtWorker __instance, ref float __result)
        {
            if (!(__instance is ThoughtWorker_Pain)) return;
            if (!p.HasTrait(BOT_TraitDefOf.BOT_Hyperalgesia)) return;
            __result *= 2f;
        }
    }
}
