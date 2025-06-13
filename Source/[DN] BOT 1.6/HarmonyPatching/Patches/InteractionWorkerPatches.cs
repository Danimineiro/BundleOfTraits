using More_Traits.DefOfs;
using More_Traits.Extensions;
using More_Traits.HediffClass;

namespace More_Traits.HarmonyPatching.Patches;

internal sealed class InteractionWorkerPatches
{
    internal static void Interacted(Pawn initiator, Pawn recipient)
    {
        if (!CheckPawns(initiator, recipient)) return;

        BOT_AnimalWhispererInteractionHediff.AddInteraction(initiator, recipient);
    }

    private static bool CheckPawns(Pawn initiator, Pawn recipient)
    {
        if (initiator.RaceProps.Animal && recipient.HasTrait(BOT_TraitDefOf.BOT_AnimalWhisperer)) return true;
        return recipient.RaceProps.Animal && initiator.HasTrait(BOT_TraitDefOf.BOT_AnimalWhisperer);
    }
}
