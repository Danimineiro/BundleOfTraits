using RimWorld;

namespace More_Traits.DefOfs;

[DefOf]
public static class BOT_ThoughtDefOf
{
    static BOT_ThoughtDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ThoughtDefOf));
    }

    public static ThoughtDef BOT_EclecticPalateAte = null!;

    public static ThoughtDef BOT_WittnessedDeathPacifist = null!;

    public static ThoughtDef BOT_Pacifist_KilledHuman = null!;

    public static ThoughtDef BOT_Pacifist_KilledAnimal = null!;

    public static ThoughtDef BOT_NyctophobiaCantSleep = null!;

    public static ThoughtDef BOT_Narcoleptic_Awake = null!;

    public static ThoughtDef BOT_LovesSleepWellRested = null!;

    public static ThoughtDef BOT_Communal_SleptInBarracks = null!;

    public static ThoughtDef BOT_Communal_SleptInBedroom = null!;

    public static ThoughtDef BOT_Communal_Sharing = null!;

    public static ThoughtDef BOT_ComedianBadJokeMood = null!;

    public static ThoughtDef BOT_ComedianGoodJokeMood = null!;

    public static ThoughtDef BOT_ComedianBadJokeOpinion = null!;

    public static ThoughtDef BOT_ComedianGoodJokeOpinion = null!;

    public static ThoughtDef BOT_ComedianBadJokeOpinionSelf = null!;

    public static ThoughtDef BOT_ComedianGoodJokeOpinionSelf = null!;

    public static ThoughtDef BOT_SadistHurtHumanlike = null!;

    public static ThoughtDef BOT_SadistHarvestedOrgan = null!;

    public static ThoughtDef BOT_SadistWitnessedDamage = null!;

    public static ThoughtDef BOT_AnimalWhispererNuzzled = null!;
}
