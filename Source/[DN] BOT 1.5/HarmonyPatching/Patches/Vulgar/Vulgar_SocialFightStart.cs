using HarmonyLib;
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
    public static class Vulgar_SocialFightStart
    {
        public static void Postfix(ref bool __result, Pawn_InteractionsTracker __instance, InteractionDef interaction)
        {
            if (!__result) return;
            if (interaction != InteractionDefOf.Insult) return;

            Pawn recipient = Traverse.Create(__instance).Field("pawn").GetValue() as Pawn;
            __result = !recipient.HasTrait(BOT_TraitDefOf.BOT_Vulgar);
        }
    }
}
