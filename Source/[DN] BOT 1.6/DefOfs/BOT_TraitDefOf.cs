using System.Diagnostics.CodeAnalysis;

namespace More_Traits.DefOfs;

[DefOf]
public static class BOT_TraitDefOf
{
    static BOT_TraitDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(TraitDefOf));
    }

    [AllowNull] public static TraitDef BOT_Pacifist;
    [AllowNull] public static TraitDef BOT_Nyctophobia;
    [AllowNull] public static TraitDef BOT_Temperature_Love;
    [AllowNull] public static TraitDef BOT_Loves_Sleeping;
    [AllowNull] public static TraitDef BOT_Metabolism;
    [AllowNull] public static TraitDef BOT_Hyperalgesia;
    [AllowNull] public static TraitDef BOT_Apathetic;
    [AllowNull] public static TraitDef BOT_Vulgar;
    [AllowNull] public static TraitDef BOT_Communal;
    [AllowNull] public static TraitDef BOT_Comedian;
    [AllowNull] public static TraitDef BOT_Sadist;
    [AllowNull] public static TraitDef BOT_Chionophile;
    [AllowNull] public static TraitDef BOT_Gregarious;
    [AllowNull] public static TraitDef BOT_Eclectic_Palate;
    [AllowNull] public static TraitDef BOT_AnimalWhisperer;
    [AllowNull] public static TraitDef BOT_SoylentNeed;

    [AllowNull, MayRequireBiotech] public static TraitDef BOT_PressureCooker;
}
