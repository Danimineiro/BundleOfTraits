using RimWorld;

namespace More_Traits.DefOfs
{
    [DefOf]
    public static class BOT_TraitDefOf
    {
        static BOT_TraitDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(TraitDefOf));
        }

        public static TraitDef BOT_Pacifist = null!;

        public static TraitDef BOT_Nyctophobia = null!;

        public static TraitDef BOT_Temperature_Love = null!;

        public static TraitDef BOT_Loves_Sleeping = null!;

        public static TraitDef BOT_Metabolism = null!;

        public static TraitDef BOT_Hyperalgesia = null!;

        public static TraitDef BOT_Apathetic = null!;

        public static TraitDef BOT_Vulgar = null!;

        public static TraitDef BOT_Communal = null!;

        public static TraitDef BOT_Comedian = null!;

        public static TraitDef BOT_Sadist = null!;

        public static TraitDef BOT_Chionophile = null!;

        public static TraitDef BOT_Gregarious = null!;

        public static TraitDef BOT_Eclectic_Palate = null!;

        public static TraitDef BOT_AnimalWhisperer = null!;
    }
}
