using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace More_Traits.DefOfs
{
    [DefOf]
    public static class BOT_JoyKindDefOf
    {
        static BOT_JoyKindDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(JoyKindDefOf));

        public static JoyKindDef BOT_LovesSleepSleeping;
    }
}
