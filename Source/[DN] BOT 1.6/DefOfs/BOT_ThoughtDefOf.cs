using System.Diagnostics.CodeAnalysis;

namespace More_Traits.DefOfs;

[DefOf]
public static class BOT_ThoughtDefOf
{
    static BOT_ThoughtDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ThoughtDefOf));
    }

    [AllowNull] public static ThoughtDef BOT_EclecticPalateAte;
    [AllowNull] public static ThoughtDef BOT_WittnessedDeathPacifist;
    [AllowNull] public static ThoughtDef BOT_Pacifist_KilledHuman;
    [AllowNull] public static ThoughtDef BOT_Pacifist_KilledAnimal;
    [AllowNull] public static ThoughtDef BOT_NyctophobiaCantSleep;
    [AllowNull] public static ThoughtDef BOT_Narcoleptic_Awake;
    [AllowNull] public static ThoughtDef BOT_LovesSleepWellRested;
    [AllowNull] public static ThoughtDef BOT_Communal_SleptInBarracks;
    [AllowNull] public static ThoughtDef BOT_Communal_SleptInBedroom;
    [AllowNull] public static ThoughtDef BOT_Communal_Sharing;
    [AllowNull] public static ThoughtDef BOT_ComedianBadJokeMood;
    [AllowNull] public static ThoughtDef BOT_ComedianGoodJokeMood;
    [AllowNull] public static ThoughtDef BOT_ComedianBadJokeOpinion;
    [AllowNull] public static ThoughtDef BOT_ComedianGoodJokeOpinion;
    [AllowNull] public static ThoughtDef BOT_ComedianBadJokeOpinionSelf;
    [AllowNull] public static ThoughtDef BOT_ComedianGoodJokeOpinionSelf;
    [AllowNull] public static ThoughtDef BOT_SadistHurtHumanlike;
    [AllowNull] public static ThoughtDef BOT_SadistHarvestedOrgan;
    [AllowNull] public static ThoughtDef BOT_SadistWitnessedDamage;
    [AllowNull] public static ThoughtDef BOT_SoylentNeed_AteNutrientPasteMeal;
    [AllowNull] public static ThoughtDef BOT_AnimalWhispererNuzzled;
    [AllowNull] public static ThoughtDef BOT_AnimalWhispererAnimalCount;
    [AllowNull] public static ThoughtDef BOT_AnimalWhisperer_BondedAnimalMaster;
    [AllowNull] public static ThoughtDef BOT_AnimalWhisperer_NotBondedAnimalMaster;
}
