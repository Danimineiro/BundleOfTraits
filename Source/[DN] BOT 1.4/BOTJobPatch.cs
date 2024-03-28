using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace More_Traits
{
	[HarmonyPatch(typeof(Toils_LayDown), "LayDown")]
	class LayDownToilPatch
	{
		public static void Postfix(Toil __result, TargetIndex bedOrRestSpotIndex, bool hasBed, bool lookForOtherJobs, bool canSleep = true, bool gainRestAndHealth = true, PawnPosture noBedLayingPosture = PawnPosture.LayingOnGroundNormal)
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
	class JobPatch
	{
		public static void Prefix(Pawn ___pawn, Job newJob, JobCondition lastJobEndCondition)
		{
			if (___pawn.RaceProps.Animal || ___pawn.story == null || ___pawn.story.traits == null) return;

			BOTTraitsManager Manager = Current.Game.GetComponent<BOTTraitsManager>();

			//Save the amount of rest a pawn with the Loves_Sleep trait has in order to later calculate how much recreation they should gain from that
			if (newJob.def == JobDefOf.LayDown && newJob.targetA.HasThing && newJob.targetA.Thing.GetType() == typeof(Building_Bed) && ___pawn.story.traits.HasTrait(BOTTraitDefOf.BOT_Loves_Sleeping))
			{
				if (!Manager.GetLoves_SleepDic().ContainsKey(___pawn))
				{
					Manager.GetLoves_SleepDic()[___pawn] = ___pawn.needs.rest.CurLevelPercentage;
				}

				return;
			}

			//Only executed if the pawn was entered in here due to Job.LayDown in a Bed
			//determines how much recreation a pawn should get
			if (Manager.GetLoves_SleepDic().ContainsKey(___pawn))
			{
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

			//Deals with sleepyheads not wanting to wake up
			if ((___pawn.CurJobDef == JobDefOf.LayDown || ___pawn.CurJobDef == JobDefOf.LayDownAwake) && newJob.def != JobDefOf.LayDown && ___pawn.story.traits.HasTrait(BOTTraitDefOf.BOT_Sleepyhead) && ___pawn.needs.rest.CurCategory == RestCategory.Rested && !Manager.GetSleepyHeadSet().Contains(___pawn))
			{
				if (Rand.Value > 0.3)
				{
					newJob = ___pawn.CurJob;
					newJob.def = JobDefOf.LayDown;
					newJob.forceSleep = true;
					___pawn.needs.rest.CurLevelPercentage = 0.30f;
					___pawn.TryGainMemory(BOTThoughtDefOf.BOT_SleepyHeadContinuesSleeping, 0);
					Manager.GetSleepyHeadSet().Add(___pawn);
				}
			}
		}
	}
}
