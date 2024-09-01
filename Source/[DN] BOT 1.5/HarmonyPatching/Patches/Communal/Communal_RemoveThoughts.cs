using More_Traits.DefOfs;
using RimWorld;
using Verse;

namespace More_Traits.HarmonyPatching.Patches.Communal;

public static class Communal_RemoveThoughts
{
    public static void Postfix(Pawn pawn)
    {
        if (pawn.needs.mood == null) return;

        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefIf(BOT_ThoughtDefOf.BOT_Communal_SleptInBarracks, HasPositiveMoodOffset);
        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefIf(BOT_ThoughtDefOf.BOT_Communal_SleptInBedroom, HasPositiveMoodOffset);
        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefIf(BOT_ThoughtDefOf.BOT_Communal_Sharing, HasPositiveMoodOffset);
    }

    private static bool HasPositiveMoodOffset(Thought_Memory thought) => thought.MoodOffset() > 0f;
}
