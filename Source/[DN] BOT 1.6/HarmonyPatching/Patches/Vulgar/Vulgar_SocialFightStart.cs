using HarmonyLib;
using More_Traits.DefOfs;
using More_Traits.Extensions;

namespace More_Traits.HarmonyPatching.Patches.Vulgar;

public static class Vulgar_SocialFightStart
{
    public static void Postfix(ref bool __result, Pawn_InteractionsTracker __instance, InteractionDef interaction)
    {
        if (!__result) return;
        if (interaction != InteractionDefOf.Insult) return;

        Pawn recipient = (Pawn)Traverse.Create(__instance).Field("pawn").GetValue();
        __result = !recipient.HasTrait(BOT_TraitDefOf.BOT_Vulgar);
    }
}
