using More_Traits.DefOfs;
using More_Traits.Extensions;
using Verse;

namespace More_Traits.HarmonyPatching.Patches.Sadist;

public static class Sadist_RemoveBodyPart
{
    public static void ApplyThoughtsPatch(Pawn pawn, Pawn billDoer)
    {
        if (billDoer.needs.mood == null) return;
        if (!billDoer.HasTrait(BOT_TraitDefOf.BOT_Sadist)) return;

        billDoer.needs.mood.thoughts.memories.TryGainMemory(BOT_ThoughtDefOf.BOT_SadistHurtHumanlike);
        billDoer.needs.mood.thoughts.memories.TryGainMemory(BOT_ThoughtDefOf.BOT_SadistHarvestedOrgan);
    }
}
