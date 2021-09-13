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
		/// <param name="threat">The main threat the pawn is facing</param>
		/// <param name="distance">An int describing the distance that is to be fled</param>
		/// <param name="threats">A list of things the pawn should avoid</param>
		/// <param name="stayWhenNowhereToGo">In case the vanilla GetFleeDest can't find anywhere to go, should the pawn simply stay where it is</param>
		public static void MakeFlee(this Pawn pawn, Thing threat, int distance, List<Thing> threats, bool stayWhenNowhereToGo = false)
		{
			MakeFlee(pawn, threat, new IntVec2(distance, distance), threats, stayWhenNowhereToGo);
		}

		/// <summary>
		///		This function attempts to create a fleeing job using the given parameters, and automatically attaches it to a pawn
		/// </summary>
		/// <param name="pawn">The Pawn that is made to flee</param>
		/// <param name="threat">The main threat the pawn is facing</param>
		/// <param name="minDistance">An int describing the min distance that is to be fled</param>
		/// <param name="maxDistance">An int describing the max distance that is to be fled</param>
		/// <param name="threats">A list of things the pawn should avoid</param>
		/// <param name="stayWhenNowhereToGo">In case the vanilla GetFleeDest can't find anywhere to go, should the pawn simply stay where it is</param>
		public static void MakeFlee(this Pawn pawn, Thing threat, int minDistance, int maxDistance, List<Thing> threats, bool stayWhenNowhereToGo = false)
		{
			MakeFlee(pawn, threat, new IntVec2(minDistance, maxDistance), threats, stayWhenNowhereToGo);
		}

		/// <summary>
		///		This function attempts to create a fleeing job using the given parameters, and automatically attaches it to a pawn
		/// </summary>
		/// <param name="pawn">The Pawn that is made to flee</param>
		/// <param name="threat">The main threat the pawn is facing</param>
		/// <param name="distance">A Vector describing the max and min distance that is to be fled</param>
		/// <param name="threats">A list of things the pawn should avoid</param>
		/// <param name="stayWhenNowhereToGo">In case the vanilla GetFleeDest can't find anywhere to go, should the pawn simply stay where it is</param>
		public static void MakeFlee(this Pawn pawn, Thing threat, IntVec2 distance, List<Thing> threats, bool stayWhenNowhereToGo = false)
		{
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
				intVec3 = CellFinderLoose.GetFleeDest(pawn, threats, Rand.Range(distance.x, distance.z));
			}
			if (intVec3 == pawn.Position && !stayWhenNowhereToGo)
			{
				//Find a random place to flee to because there was nowhere to go
				intVec3 = GenRadial.RadialCellsAround(pawn.Position, (float)distance.x, (float)(distance.z)).RandomElement();
			}
			if (intVec3 != pawn.Position)
			{
				job = JobMaker.MakeJob(JobDefOf.Flee, intVec3, threat);
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
	}
}
