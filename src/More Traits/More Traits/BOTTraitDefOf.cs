using RimWorld;

namespace More_Traits
{
	[DefOf]
	public static class BOTTraitDefOf
	{
		static BOTTraitDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(TraitDefOf));
		}

		public static TraitDef BOT_Misanthrope;

		public static TraitDef BOT_Dysgeusia;

		public static TraitDef BOT_Eclectic_Palate;

		public static TraitDef BOT_Pacifist;

		public static TraitDef BOT_Narcoleptic;

		public static TraitDef BOT_Pyrophobia;

		public static TraitDef BOT_Nyctophobia;

		public static TraitDef BOT_Sleepyhead;

		public static TraitDef BOT_GunKata;

		public static TraitDef BOT_Loves_Sleeping;

		public static TraitDef BOT_Cynical;

		public static TraitDef BOT_StrongBody;

		public static TraitDef BOT_WeakBody;

		public static TraitDef BOT_Metabolism;

		public static TraitDef BOT_Temperature_Love;
	}

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
	}

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
}
