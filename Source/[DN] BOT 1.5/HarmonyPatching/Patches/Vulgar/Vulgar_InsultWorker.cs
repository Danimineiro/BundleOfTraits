using More_Traits.DefOfs;
using More_Traits.Extensions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.HarmonyPatching.Patches.Vulgar
{
    public static class Vulgar_InsultWorker
    {
        public static void RandomSelectionWeight(ref float __result, Pawn initiator, Pawn recipient)
        {
            if (initiator.HasTrait(BOT_TraitDefOf.BOT_Vulgar))
            {
                __result = 5f * NegativeInteractionUtility.NegativeInteractionChanceFactor(initiator, recipient) * 0.007f;
            }
        }
    }
}
