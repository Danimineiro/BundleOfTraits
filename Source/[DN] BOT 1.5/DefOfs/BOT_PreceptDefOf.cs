using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace More_Traits.DefOfs
{
    [DefOf]
    public static class BOT_PreceptDefOf
    {
        static BOT_PreceptDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PreceptDefOf));

        [MayRequireIdeology]
        public static PreceptDef Pain_Idealized;
    }
}
