using RimWorld;
using Verse;

namespace More_Traits.DefOfs
{
    [DefOf]
    public static class BOT_HediffDefOf
    {
        static BOT_HediffDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(HediffDefOf));

        public static HediffDef Burn = null!;

        public static HediffDef BOT_ClaustrophobicBreakdown = null!;
    }
}
