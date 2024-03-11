using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace More_Traits
{
	[HarmonyPatch(typeof(Toils_LayDown), "LayDown")]
	internal static class LayDownToilPatch
	{
		internal static void Postfix(Toil __result, TargetIndex bedOrRestSpotIndex, bool hasBed, bool lookForOtherJobs, bool canSleep = true, bool gainRestAndHealth = true, PawnPosture noBedLayingPosture = PawnPosture.LayingOnGroundNormal)
		{
			__result.AddFailCondition(delegate ()
			{
				if (__result.actor != null && __result.actor.HasTrait(BOTTraitDefOf.BOT_Nyctophobia))
				{
					//Determine if a Nyctophobic person can sleep
					Pawn actor = __result.actor;
					Map map = actor.Map;
					HashSet<Pawn> set = Current.Game.GetComponent<BOTTraitsManager>().GetNyctophobes();

					if (actor.Position.InBounds(map) && map.glowGrid.GameGlowAt(actor.Position) < 0.3f && actor.needs.rest.CurCategory != RestCategory.Exhausted && actor.CurJobDef == JobDefOf.LayDown && !set.Contains(actor))
					{
						Log.Message("Too dark for: " + actor.Name);
						actor.TryGainMemory(BOTThoughtDefOf.BOT_NyctophobiaCantSleep, 0);
						Messages.Message("BOTNyctophobeCantSleep".Translate(actor.LabelShort, actor), actor, MessageTypeDefOf.NegativeEvent);

						actor.jobs.StartJob(new Job(JobDefOf.LayDownAwake, actor.jobs.curJob.targetA), JobCondition.InterruptForced);
						set.Add(actor);

						return true;
					}

					if (actor.needs.rest.CurCategory == RestCategory.Exhausted && Current.Game.GetComponent<BOTTraitsManager>().GetNyctophobes().Contains(actor) && actor.CurJobDef == JobDefOf.LayDownAwake)
					{
						actor.jobs.StartJob(new Job(JobDefOf.LayDown, actor.jobs.curJob.targetA), JobCondition.InterruptForced);

						return true;
					}
				} 
				return false;
			});

			__result.AddFinishAction(delegate ()
			{
				if (__result.actor != null && __result.actor.HasTrait(BOTTraitDefOf.BOT_Nyctophobia))
				{
					__result.actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(BOTThoughtDefOf.BOT_NyctophobiaCantSleep);

					HashSet<Pawn> set = Current.Game.GetComponent<BOTTraitsManager>().GetNyctophobes();

					if (set.Contains(__result.actor) && __result.actor.needs.rest.CurCategory != RestCategory.Exhausted)
					{
						set.Remove(__result.actor);
					}
				}
			});
		}
	}


	/// <summary>
	///		This class patches the startjob function to modify the behaviour of sleep related traits
	/// </summary>
	[HarmonyPatch(typeof(Pawn_JobTracker), "StartJob")]
	internal static class JobPatch
	{
		internal static void Prefix(Pawn ___pawn, ref Job newJob)
        {
            if (___pawn.AnimalOrWildMan()) return;

            BOTTraitsManager Manager = Current.Game.GetComponent<BOTTraitsManager>();
            AddPawnToSleepDictionary(___pawn, Manager, newJob);

            //Only executed if the pawn was entered in here due to Job.LayDown in a Bed
            //determines how much recreation a pawn should get
            CalcSleepJoy(___pawn, Manager);

            //Deals with sleepyheads not wanting to wake up
            MaybeContinueSleeping(___pawn, ref newJob, Manager);
        }

        private static void MaybeContinueSleeping(Pawn pawn, ref Job newJob, BOTTraitsManager Manager)
        {
            if (!pawn.HasTrait(BOTTraitDefOf.BOT_Sleepyhead)) return;
            if (!(pawn.CurJobDef == JobDefOf.LayDown || pawn.CurJobDef == JobDefOf.LayDownAwake)) return;
            if (newJob.def == JobDefOf.LayDown || newJob.def == JobDefOf.LayDownAwake) return;
            if (pawn.needs.rest.CurCategory != RestCategory.Rested || Manager.GetSleepyHeadSet().Contains(pawn)) return;
            if (Rand.Value <= 0.3f) return;
            
            newJob = pawn.CurJob;
            newJob.def = JobDefOf.LayDown;
            newJob.forceSleep = true;
            pawn.needs.rest.CurLevelPercentage = 0.30f;
            pawn.TryGainMemory(BOTThoughtDefOf.BOT_SleepyHeadContinuesSleeping, 0);
            Manager.GetSleepyHeadSet().Add(pawn);
        }

        private static void CalcSleepJoy(Pawn ___pawn, BOTTraitsManager Manager)
        {
            if (!Manager.GetLoves_SleepDic().ContainsKey(___pawn)) return;

            float initialRestPercent = Manager.GetLoves_SleepDic()[___pawn];
            float currentRestPercent = ___pawn.needs.rest.CurLevelPercentage;
            float recreationGainPercent = (currentRestPercent - initialRestPercent) * 0.3f;

            ___pawn.needs.joy.GainJoy(recreationGainPercent, BOTJoyKindDefOf.BOT_LovesSleepSleeping);

            if (currentRestPercent > 0.9)
            {
                ___pawn.TryGainMemory(BOTThoughtDefOf.BOT_LovesSleepWellRested, 0);
            }

            Manager.GetLoves_SleepDic().Remove(___pawn);
        }

        /// <summary>
        ///		Save the amount of rest a pawn with the Loves_Sleep trait has in order to later calculate how much recreation they should gain from that
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="Manager"></param>
        /// <param name="newJob"></param>
        private static void AddPawnToSleepDictionary(Pawn pawn, BOTTraitsManager Manager, Job newJob)
        {
            if (newJob.def != JobDefOf.LayDown) return;
            if (!newJob.targetA.HasThing) return;
            if (newJob.targetA.Thing.GetType() != typeof(Building_Bed)) return;
            if (!pawn.HasTrait(BOTTraitDefOf.BOT_Loves_Sleeping)) return;

            if (!Manager.GetLoves_SleepDic().ContainsKey(pawn))
            {
                Manager.GetLoves_SleepDic()[pawn] = pawn.needs.rest.CurLevelPercentage;
            }

            return;
        }
    }
}
