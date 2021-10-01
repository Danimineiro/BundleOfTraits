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
			float num = p.AmbientTemperature - p.GetStatValue(StatDefOf.ComfyTemperatureMax, true);

			if (p.story.traits.DegreeOfTrait(BOTTraitDefOf.BOT_Temperature_Love) == -1)
			{
				//Loves the heat
				if (p.AmbientTemperature > 25f && num < 10f)
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
			if (num <= 0f)
			{
				return ThoughtState.Inactive;
			}
			int num2;
			if (num < 10f)
			{
				num2 = 0;
			}
			else if (num < 20f)
			{
				num2 = 1;
			}
			else if (num < 30f)
			{
				num2 = 2;
			}
			else
			{
				num2 = 3;
			}
			if (ModsConfig.IdeologyActive && p.Ideo.HasPrecept(PreceptDefOf.Temperature_Tough))
			{
				num2 -= 2;
			}
			if (num2 >= 0)
			{
				return ThoughtState.ActiveAtStage(num2);
			}
			return ThoughtState.Inactive;
		}
	}

	public class BOT_ThoughtWorker_Cold : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			float num = p.GetStatValue(StatDefOf.ComfyTemperatureMin, true) - p.AmbientTemperature;

			if (p.story.traits.DegreeOfTrait(BOTTraitDefOf.BOT_Temperature_Love) == -1)
			{
				//Loves the heat
			}
			else if (p.story.traits.DegreeOfTrait(BOTTraitDefOf.BOT_Temperature_Love) == 1)
			{
				//Loves the cold
				if (p.AmbientTemperature < 5f && num < 10f)
				{
					return ThoughtState.ActiveAtStage(4);
				}
				return false;
			}
			else
			{
				return false;
			}
			if (num <= 0f)
			{
				return ThoughtState.Inactive;
			}
			int num2;
			if (num < 10f)
			{
				num2 = 0;
			}
			else if (num < 20f)
			{
				num2 = 1;
			}
			else if (num < 30f)
			{
				num2 = 2;
			}
			else
			{
				num2 = 3;
			}
			if (ModsConfig.IdeologyActive && p.Ideo.HasPrecept(PreceptDefOf.Temperature_Tough))
			{
				num2 -= 2;
			}
			if (num2 >= 0)
			{
				return ThoughtState.ActiveAtStage(num2);
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
