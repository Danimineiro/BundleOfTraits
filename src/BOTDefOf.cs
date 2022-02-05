using RimWorld;
using Verse;

namespace More_Traits
{
	/// <summary>
	///		Class <c>BOTTraitDefOf</c> defines <c>TraitDef</c>s
	/// </summary>
	[DefOf]
	public static class BOTTraitDefOf
	{
        static BOTTraitDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(TraitDefOf));

        public static TraitDef BOT_Misanthrope;

		public static TraitDef BOT_Dysgeusia;

		public static TraitDef BOT_Eclectic_Palate;

		public static TraitDef BOT_Pacifist;

		public static TraitDef BOT_Narcoleptic;

		public static TraitDef BOT_Pyrophobia;

		public static TraitDef BOT_Nyctophobia;

		public static TraitDef BOT_Sleepyhead;

		public static TraitDef BOT_Loves_Sleeping;

		public static TraitDef BOT_StrongBody;

		public static TraitDef BOT_WeakBody;

		public static TraitDef BOT_Metabolism;

		public static TraitDef BOT_Temperature_Love;

		public static TraitDef BOT_Apathetic;

		public static TraitDef BOT_Entomophobia;

		public static TraitDef BOT_Entomophagous;

		public static TraitDef BOT_Gregarious;

		public static TraitDef BOT_Chionophile;

		public static TraitDef BOT_Communal;

		public static TraitDef BOT_Sadist;

		public static TraitDef BOT_Vulgar;

		public static TraitDef BOT_Comedian;

		public static TraitDef BOT_CongenitalAnalgesia;

		public static TraitDef Immunity;
	}

	/// <summary>
	///		Class <c>BOTThoughtDefOf</c> defines <c>ThoughtDef</c>s
	/// </summary>
	[DefOf]
	public static class BOTThoughtDefOf
	{
		static BOTThoughtDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(ThoughtDefOf));
		}

		public static ThoughtDef BOT_Pacifist_KilledAnimal;

		public static ThoughtDef BOT_Pacifist_KilledHuman;

		public static ThoughtDef BOT_WittnessedDeathPacifist;

		public static ThoughtDef BOT_PyrophobiaHoldingIncendiary;

		public static ThoughtDef BOT_EclecticPalateAte;

		public static ThoughtDef BOT_PyrophobicBurned;

		public static ThoughtDef BOT_PyrophobicNearFire;

		public static ThoughtDef BOT_PyrophobicOnFire;

		public static ThoughtDef BOT_LovesSleepWellRested;

		public static ThoughtDef BOT_NyctophobiaCantSleep;
		
		public static ThoughtDef BOT_SleepyHeadContinuesSleeping;

		public static ThoughtDef BOT_SleepyHeadForcefullyWokenUp;
		
		public static ThoughtDef BOT_SleepyHeadStopsSleeping;

		public static ThoughtDef BOT_Communal_SleptInBarracks;

		public static ThoughtDef BOT_Communal_SleptInBedroom;

		public static ThoughtDef BOT_CommunalSharing;

		public static ThoughtDef BOT_WitnessedDamageSadist;

		public static ThoughtDef BOT_HurtHumanlikeSadist;

		public static ThoughtDef BOT_HarvestedOrgan_Sadist;

		public static ThoughtDef BOT_ComedianGoodJokeMood;

		public static ThoughtDef BOT_ComedianBadJokeMood;

		public static ThoughtDef BOT_ComedianGoodJokeOpinion;

		public static ThoughtDef BOT_ComedianBadJokeOpinion;

		public static ThoughtDef BOT_ComedianGoodJokeOpinionSelf;

		public static ThoughtDef BOT_ComedianBadJokeOpinionSelf;
	}


	/// <summary>
	///		Class <c>BOTPreceptDefOf</c> defines <c>PreceptDef</c>s
	///		As of right now the mod doesn't come with it's own precepts, so the only precept here is one not present in Vanilla, but required in code
	/// </summary>
	[DefOf]
	public static class BOTPreceptDefOf
	{
		static BOTPreceptDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(PreceptDefOf));
		}

		[MayRequireIdeology]
		public static PreceptDef Pain_Idealized;
	}

	/// <summary>
	///		Class <c>BOTJoyKindDefOf</c> defines <c>JoyKindDef</c>s
	/// </summary>
	[DefOf]
	public static class BOTJoyKindDefOf
	{
		static BOTJoyKindDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(JoyKindDefOf));
		}

		public static JoyKindDef BOT_LovesSleepSleeping;
	}

	/// <summary>
	///		Class <c>BOTJoyKindDefOf</c> defines <c>HediffDef</c>s
	/// </summary>
	[DefOf]
	public static class BOTHediffDefOf
	{
		static BOTHediffDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(BOTHediffDefOf));
		}

		public static HediffDef BOT_ClaustrophobicBreakdown;

		public static HediffDef BOT_CongenitalAnalgesiaPainReducer;
	}
}
