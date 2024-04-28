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
    public static class BOT_ThinkTreeDefOf
    {
        static BOT_ThinkTreeDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(ThinkTreeDefOf));

        public static ThinkTreeDef Bot_FleeingBehaviour;
    }
}
