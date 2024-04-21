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

        public static ThoughtDef BOT_Communal_SleptInBarracks;

        public static ThoughtDef BOT_Communal_SleptInBedroom;

        public static ThoughtDef BOT_Communal_Sharing;

        public static ThoughtDef BOT_ComedianBadJokeMood;

        public static ThoughtDef BOT_ComedianGoodJokeMood;

        public static ThoughtDef BOT_ComedianBadJokeOpinion;

        public static ThoughtDef BOT_ComedianGoodJokeOpinion;

        public static ThoughtDef BOT_ComedianBadJokeOpinionSelf;

        public static ThoughtDef BOT_ComedianGoodJokeOpinionSelf;

        public static ThoughtDef BOT_SadistHurtHumanlike;

        public static ThoughtDef BOT_SadistHarvestedOrgan;

        public static ThoughtDef BOT_SadistWitnessedDamage;
    }
}
