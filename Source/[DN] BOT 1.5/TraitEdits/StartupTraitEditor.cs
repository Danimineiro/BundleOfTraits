using More_Traits.DefOfs;
using RimWorld;
using Verse;

namespace More_Traits.TraitEdits;

[StaticConstructorOnStartup]
public static class StartupTraitEditor
{
    static StartupTraitEditor()
    {
        BOT_TraitDefOf.BOT_Apathetic.DataAtDegree(0).disallowedInspirations.AddRange(DefDatabase<InspirationDef>.AllDefsListForReading);
        BOT_TraitDefOf.BOT_Apathetic.conflictingPassions.AddRange(DefDatabase<SkillDef>.AllDefsListForReading);
    }
}
