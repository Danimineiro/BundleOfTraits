using System.Collections.Generic;
using Verse;
using RimWorld;
using Verse.AI;

namespace More_Traits
{
	/// <summary>
	/// The purpose of this class is to store general functions which may see more widespread use later on
	/// </summary>
	public static class BOTUtils
	{
		/// <summary>
		///		This function attempts to create a fleeing job using the given parameters, and automatically attaches it to a pawn
		/// </summary>
		/// <param name="pawn">The Pawn that is made to flee</param>
		/// <param name="param">The main threat the pawn is facing</param>
		public static void MakeFlee(this Pawn pawn, BOTFleeParams param)
		{
			IntVec2 distance = param.Distance;

			if (distance.x > distance.z)
			{
				Log.Warning("MakeFlee was called where minDistance was bigger than maxDistance. Min: " + distance.x + " Max: " + distance.z + "!");
				distance.z = distance.x;
			}

			Job job = null;
			IntVec3 intVec3;
			if (pawn.CurJob != null && pawn.CurJob.def == JobDefOf.Flee)
			{
				//Continue Fleeing
				intVec3 = pawn.CurJob.targetA.Cell;
			}
			else
			{
				//Find a place to flee to
				intVec3 = CellFinderLoose.GetFleeDest(pawn, param.Threats, Rand.Range(distance.x, distance.z));
			}
			if (intVec3 == pawn.Position && !param.StayWhenNowhereToGo)
			{
				//Find a random place to flee to because there was nowhere to go
				intVec3 = GenRadial.RadialCellsAround(pawn.Position, distance.x, distance.z).RandomElement();
			}
			if (intVec3 != pawn.Position)
			{
				job = JobMaker.MakeJob(JobDefOf.Flee, intVec3, param.Threat);
			}
			if (job != null)
			{
				pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, false);
			}
		}

		/// <summary>
		///		This function works opposite the heal function for hediffs, but it checks if an applied injury would be too strong and if it is, it doesn't apply.
		/// </summary>
		/// <param name="toInjure">The hediff to be injured</param>
		/// <param name="amount">The amount of damage to be done</param>
		public static void Injure(this Hediff toInjure, float amount)
		{
			if ((toInjure.def.lethalSeverity >= 0 && toInjure.Severity + amount > toInjure.def.lethalSeverity) || toInjure.Severity + amount > toInjure.Part.def.GetMaxHealth(toInjure.pawn))
			{
				return;
			}
			
			toInjure.Severity += amount;
			toInjure.pawn.health.Notify_HediffChanged(toInjure);
		}

		public static void TryGainMemory(this Pawn pawn, ThoughtDef thoughtDef, int forcedLevel)
        {
			if (pawn.needs.mood != null) pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(thoughtDef, forcedLevel));
		}

		public static bool IsFullyHealed(this Pawn pawn) => !pawn.health.HasHediffsNeedingTendByPlayer(false) && !HealthAIUtility.ShouldSeekMedicalRest(pawn) && !pawn.health.hediffSet.HasTendedAndHealingInjury();

		public static bool IsAsleep(this Pawn pawn) => pawn.jobs?.curDriver?.asleep == false;

		public static bool HasTrait(this Pawn pawn, TraitDef traitDef) => pawn?.story?.traits?.HasTrait(traitDef) ?? false;

		public static bool HasSightOfAnyIn(this Pawn pawn, List<Thing> dangers)
		{
			if (pawn?.Map == null || !pawn.Position.IsValid || dangers.Count == 0) return false;
			return dangers.Any(danger => danger.Position.IsValid && GenSight.LineOfSight(pawn.Position, danger.Position, pawn.Map));
        }

		public static int StageOfTwenty(int n)
        {
			if (n > 20) n = 20;
			return n > 0 ? n / 2 + 1 : 0;
		}
	}
}
