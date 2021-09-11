using System.Collections.Generic;
using Verse;
using RimWorld;
using Verse.AI;

namespace More_Traits
{
	public static class BOTUtils
	{
		public static void MakeFlee(Pawn pawn, Thing threat, int distance, List<Thing> threats, bool stayWhenNowhereToGo = false)
		{
			MakeFlee(pawn, threat, new IntVec2(distance, distance), threats, stayWhenNowhereToGo);
		}

		public static void MakeFlee(Pawn pawn, Thing threat, int minDistance, int maxDistance, List<Thing> threats, bool stayWhenNowhereToGo = false)
		{
			MakeFlee(pawn, threat, new IntVec2(minDistance, maxDistance), threats, stayWhenNowhereToGo);
		}

		public static void MakeFlee(Pawn pawn, Thing threat, IntVec2 distance, List<Thing> threats, bool stayWhenNowhereToGo = false)
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
	}
}
