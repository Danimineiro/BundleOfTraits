using System;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace More_Traits
{
	public class ThoughtWorker_Misanthrope : ThoughtWorker
	{
		protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
		{
			return p.RaceProps.Humanlike && otherPawn.RaceProps.Humanlike && RelationsUtility.PawnsKnowEachOther(p, otherPawn);
		}
	}

	public class ThoughtWorker_IsCarryingWeapon : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			return p.equipment.Primary != null;
		}
	}

	public class BOT_ThoughtWorker_PyrophobicOnFire : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			return p.IsBurning();
		}
	}

	public class BOT_ThoughtWorker_Hot : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			float temperature = p.AmbientTemperature - p.GetStatValue(StatDefOf.ComfyTemperatureMax, true);

			if (p.story.traits.DegreeOfTrait(BOTTraitDefOf.BOT_Temperature_Love) == -1)
			{
				//Loves the heat
				if (p.AmbientTemperature > 25f && temperature < 10f)
				{
					return ThoughtState.ActiveAtStage(4);
				}
				return false;
			} else if (p.story.traits.DegreeOfTrait(BOTTraitDefOf.BOT_Temperature_Love) == 1)
			{
				//Loves the cold
			} else
			{
				return false;
			}

			if (temperature <= 0f) return ThoughtState.Inactive;

			int thoughtstage = Math.Min(3, ((int) temperature) / 10);
			if (ModsConfig.IdeologyActive && p.Ideo.HasPrecept(PreceptDefOf.Temperature_Tough))
			{
				thoughtstage -= 2;
			}
			if (thoughtstage >= 0)
			{
				return ThoughtState.ActiveAtStage(thoughtstage);
			}
			return ThoughtState.Inactive;
		}
	}

	public class BOT_ThoughtWorker_Cold : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			float temperature = p.GetStatValue(StatDefOf.ComfyTemperatureMin, true) - p.AmbientTemperature;

			if (p.story.traits.DegreeOfTrait(BOTTraitDefOf.BOT_Temperature_Love) == -1)
			{
				//Loves the heat
			}
			else if (p.story.traits.DegreeOfTrait(BOTTraitDefOf.BOT_Temperature_Love) == 1)
			{
				//Loves the cold
				if (p.AmbientTemperature < 5f && temperature < 10f)
				{
					return ThoughtState.ActiveAtStage(4);
				}
				return false;
			}
			else
			{
				return false;
			}

			if (temperature <= 0f) return ThoughtState.Inactive;

			int thoughtStage = Math.Min(3, ((int) temperature) / 10);
			if (ModsConfig.IdeologyActive && p.Ideo.HasPrecept(PreceptDefOf.Temperature_Tough))
			{
				thoughtStage -= 2;
			}
			if (thoughtStage >= 0)
			{
				return ThoughtState.ActiveAtStage(thoughtStage);
			}
			return ThoughtState.Inactive;
		}
	}

	public class BOT_ThoughtWorker_PyrophobicBurned : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Burn) != null)
			{
				if (ModsConfig.IdeologyActive && p.Ideo.HasPrecept(BOTPreceptDefOf.Pain_Idealized))
				{
					return ThoughtState.ActiveAtStage(1);
				}
				return ThoughtState.ActiveAtStage(0);
			}
			return false;
		}
	}

	public class ThoughtWorker_GregariousCompany : ThoughtWorker
	{
		public override string PostProcessDescription(Pawn p, string description)
		{
			description = description.Formatted(BOTGregariousCompanyCounter.GetCountFor(p).Named("NUMBER"));
			return base.PostProcessDescription(p, description);
		}

		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			int company = BOTGregariousCompanyCounter.GetCountFor(p);

			return ThoughtState.ActiveAtStage(BOTUtils.StageOfTwenty(company));
		}
	}

	public class BOT_ThoughtWorker_PluviophileLikesRain : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (!p.Spawned) return false;

			return p.Map.weatherManager.RainRate > 0.25 && p.Map.weatherManager.SnowRate < 0.25;
		}
	}

	public class BOT_ThoughtWorker_ChionophileLikesSnow : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (!p.Spawned) return false;

			return p.Map.weatherManager.RainRate > 0.25 && p.Map.weatherManager.SnowRate > 0.25;
		}
	}

	public class BOT_ThoughtWorker_MoodyModifier : ThoughtWorker
	{
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            switch (p.needs.mood.CurLevelPercentage)
			{
				case float n when n > 0.8f:
					return ThoughtState.ActiveAtStage(3);

				case float n when n > 0.6f && n < 0.8f:
					return ThoughtState.ActiveAtStage(2);

				case float n when n > 0.2f && n < 0.4f:
					return ThoughtState.ActiveAtStage(1);

				case float n when n < 0.2f:
					return ThoughtState.ActiveAtStage(0);
			}

			return false;
        }
    }

	public class BOT_ThoughtWorker_EarlyBirdTimeThought : ThoughtWorker
    {
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (p.Awake() && GenLocalDate.HourInteger(p) >= 6 && GenLocalDate.HourInteger(p) < 13)
			{
				return ThoughtState.ActiveAtStage(1);
			}
			if (p.Awake() && GenLocalDate.HourInteger(p) >= 17)
			{
				return ThoughtState.ActiveAtStage(0);
			}

			return false;
		}
	}
}
