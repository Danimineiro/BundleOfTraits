using RimWorld;
using Verse;

namespace More_Traits.DefOfs;

[DefOf]
public static class BOT_ThinkTreeDefOf
{
    static BOT_ThinkTreeDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(ThinkTreeDefOf));

    public static ThinkTreeDef Bot_FleeingBehaviour = null!;
}
