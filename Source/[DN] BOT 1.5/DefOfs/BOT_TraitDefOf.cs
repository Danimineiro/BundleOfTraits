using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace More_Traits.DefOfs
{
    [DefOf]
    public static class BOT_TraitDefOf
    {
        static BOT_TraitDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(TraitDefOf));
        }

        public static TraitDef BOT_Pacifist;

        public static TraitDef BOT_Nyctophobia;

        public static TraitDef BOT_Temperature_Love;
    }
}
