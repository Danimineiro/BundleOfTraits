using More_Traits.DefOfs;
using More_Traits.Extensions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace More_Traits.HarmonyPatching.Patches.Apathetic
{
    public static class Apathetic_SkillUI
    {
        private static bool descriptionIsForApatheticPawn = false;

        public static void GetSkillDescription_Marker(SkillRecord sk)
        {
            descriptionIsForApatheticPawn = sk.Pawn.HasTrait(BOT_TraitDefOf.BOT_Apathetic);
        }

        public static bool GetLearningFactor_Prefix(ref float __result)
        {
            if (!descriptionIsForApatheticPawn) return true;

            __result = .75f;
            return false;
        }
    }
}
