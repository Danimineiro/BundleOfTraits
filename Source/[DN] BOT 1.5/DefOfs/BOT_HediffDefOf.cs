using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.DefOfs
{
    [DefOf]
    public static class BOT_HediffDefOf
    {
        static BOT_HediffDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(HediffDefOf));

        public static HediffDef Burn;
    }
}
