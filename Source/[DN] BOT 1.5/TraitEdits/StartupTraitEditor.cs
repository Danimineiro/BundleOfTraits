using More_Traits.DefOfs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.TraitEdits
{
    [StaticConstructorOnStartup]
    public static class StartupTraitEditor
    {
        static StartupTraitEditor()
        {
            BOT_TraitDefOf.BOT_Apathetic.DataAtDegree(0).disallowedInspirations = DefDatabase<InspirationDef>.AllDefsListForReading;
            BOT_TraitDefOf.BOT_Apathetic.conflictingPassions = DefDatabase<SkillDef>.AllDefsListForReading;
        }
    }
}
