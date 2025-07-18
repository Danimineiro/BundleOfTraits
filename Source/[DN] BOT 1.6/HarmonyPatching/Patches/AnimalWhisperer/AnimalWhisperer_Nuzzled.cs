﻿using More_Traits.DefOfs;
using More_Traits.Extensions;

namespace More_Traits.HarmonyPatching.Patches.AnimalWhisperer;

internal static class AnimalWhisperer_Nuzzled
{
    internal static void InteractionWorker_Nuzzle_Interacted(Pawn initiator, Pawn recipient)
    {
        if (!initiator.RaceProps.Animal) return;
        if (!recipient.HasTrait(BOT_TraitDefOf.BOT_AnimalWhisperer)) return;

        Thought_Memory newThought = (Thought_Memory)ThoughtMaker.MakeThought(BOT_ThoughtDefOf.BOT_AnimalWhispererNuzzled);
        recipient.needs.mood?.thoughts.memories.TryGainMemory(newThought, null);
    }
}
