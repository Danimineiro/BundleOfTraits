using More_Traits.DefOfs;
using More_Traits.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.HarmonyPatching.Patches.AnimalWhisperer
{
    internal class AnimalWhisperer_TamingFail
    {
        internal static bool CheckStartMentalStateBecauseRecruitAttempted(Pawn tamer, ref bool __result)
        {
            return __result = !tamer.HasTrait(BOT_TraitDefOf.BOT_AnimalWhisperer);
        }
    }
}
