using More_Traits.DefOfs;
using More_Traits.Extensions;
using More_Traits.WorldComps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace More_Traits.HarmonyPatching.Patches.Nyctophobe
{
    internal class Nyctophobe_CanNotSleep
    {
        internal static Toil NoSleepToil(JobDriver_LayDown jobDriver)
        {
            if (!jobDriver.CanRest) return null;
            if (!jobDriver.CanSleep) return null;
            if (jobDriver.Bed == null) return null;

            Pawn pawn = jobDriver.pawn;
            if (!pawn.HasTrait(BOT_TraitDefOf.BOT_Nyctophobia)) return null;

            Toil toil = ToilMaker.MakeToil(nameof(NoSleepToil));
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.FailOnBedNoLongerUsable(TargetIndex.A);

            void initAction()
            {
                Pawn actor = toil.actor;
                Job curJob = actor.CurJob;
                JobDriver_LayDown driver = actor.jobs.curDriver as JobDriver_LayDown;
                Building_Bed bed = actor.CurJob.GetTarget(TargetIndex.A).Thing as Building_Bed;

                if (!bed.OccupiedRect().Contains(actor.Position))
                {
                    Log.Error($"Can't start {nameof(NoSleepToil)} because pawn is not in the bed. pawn: {actor}");
                    actor.jobs.EndCurrentJob(JobCondition.Errored);
                    return;
                }

                actor.pather.StopDead();

                if (!TooDarkFor(actor) || actor.needs.rest.CurCategory >= RestCategory.VeryTired)
                {
                    driver.ReadyForNextToil();
                    return;
                }

                driver.asleep = false;
                actor.jobs.posture = PawnPosture.LayingInBedFaceUp;
                PortraitsCache.SetDirty(actor);
                actor.TryGainMemory(BOT_ThoughtDefOf.BOT_NyctophobiaCantSleep, 0);
                Messages.Message("BOTNyctophobeCantSleep".Translate(actor.LabelShort, actor), actor, MessageTypeDefOf.NegativeEvent);
            }

            void tickAction()
            {
                Pawn actor = toil.actor;
                JobDriver_LayDown driver = actor.jobs.curDriver as JobDriver_LayDown;
                Building_Bed bed = actor.CurJob.GetTarget(TargetIndex.A).Thing as Building_Bed;

                Type boolType = typeof(bool);
                MethodInfo ApplyBedEffects = typeof(Toils_LayDown).GetMethod("ApplyBedRelatedEffects", BindingFlags.Static | BindingFlags.NonPublic);
                ApplyBedEffects.Invoke(null, new object[] { actor, bed, false, true, false });

                if (!TooDarkFor(actor) || actor.needs.rest.CurCategory >= RestCategory.VeryTired)
                {
                    driver.ReadyForNextToil();
                }
            }

            void finishAction()
            {
                toil.actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(BOT_ThoughtDefOf.BOT_NyctophobiaCantSleep);
            }

            toil.initAction = initAction;
            toil.tickAction = tickAction;
            toil.AddFinishAction(finishAction);

            return toil;
        }

        private static bool TooDarkFor(Pawn actor)
        {
            return actor.Position.InBounds(actor.Map) && actor.Map.glowGrid.GroundGlowAt(actor.Position) < 0.3f;
        }
    }
}
