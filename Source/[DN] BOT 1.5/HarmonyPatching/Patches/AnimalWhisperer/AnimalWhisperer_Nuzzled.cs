using More_Traits.DefOfs;
using RimWorld;
using Verse;

namespace More_Traits.HarmonyPatching.Patches.AnimalWhisperer
{
    internal static class AnimalWhisperer_Nuzzled
    {
        internal static void InteractionWorker_Nuzzle_Interacted(Pawn recipient)
        {
            Thought_Memory newThought = (Thought_Memory)ThoughtMaker.MakeThought(BOT_TraitDefOf.BOT_AnimalWhispererNuzzled);
            recipient.needs.mood?.thoughts.memories.TryGainMemory(newThought, null);
        }
    }
}
