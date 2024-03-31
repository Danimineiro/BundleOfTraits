using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace More_Traits.DefOfs
{
    [DefOf]
    public static class BOT_ThoughtDefOf
    {
        static BOT_ThoughtDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThoughtDefOf));
        }

        public static ThoughtDef BOT_EclecticPalateAte;

        public static ThoughtDef BOT_WittnessedDeathPacifist;

        public static ThoughtDef BOT_Pacifist_KilledHuman;

        public static ThoughtDef BOT_Pacifist_KilledAnimal;

        public static ThoughtDef BOT_NyctophobiaCantSleep;

        public static ThoughtDef BOT_Narcoleptic_Awake;

        public static ThoughtDef BOT_LovesSleepWellRested;
    }
}
