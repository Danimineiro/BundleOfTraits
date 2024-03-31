using More_Traits.DefOfs;
using More_Traits.Extensions;
using More_Traits.WorldComps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace More_Traits.HarmonyPatching.Patches
{
    public class Nyctophobe_CanNotSleep
    {
        public static void Postfix(Toil __result, TargetIndex bedOrRestSpotIndex, bool hasBed, bool lookForOtherJobs, bool canSleep = true, bool gainRestAndHealth = true, PawnPosture noBedLayingPosture = PawnPosture.LayingOnGroundNormal)
        {
            if (!(__result.actor is Pawn pawn)) return;
            if (!pawn.HasTrait(BOT_TraitDefOf.BOT_Nyctophobia)) return;

            BOT_WorldComponent savingComp = BOT_WorldComponent.Instance;

            bool newFailCondition()
            {
                //Determine if a Nyctophobic person can sleep
                Map map = pawn.Map;

                if (pawn.Position.InBounds(map) && map.glowGrid.GroundGlowAt(pawn.Position) < 0.3f && pawn.needs.rest.CurCategory != RestCategory.Exhausted && pawn.CurJobDef == JobDefOf.LayDown && !savingComp.NotifiedNyctoPawnSet.Contains(pawn))
                {
                    pawn.TryGainMemory(BOT_ThoughtDefOf.BOT_NyctophobiaCantSleep, 0);
                    Messages.Message("BOTNyctophobeCantSleep".Translate(pawn.LabelShort, pawn), pawn, MessageTypeDefOf.NegativeEvent);

                    pawn.jobs.StartJob(new Job(JobDefOf.LayDownAwake, pawn.jobs.curJob.targetA), JobCondition.InterruptForced);
                    savingComp.NotifiedNyctoPawnSet.Add(pawn);

                    return true;
                }

                if (pawn.needs.rest.CurCategory == RestCategory.Exhausted && savingComp.NotifiedNyctoPawnSet.Contains(pawn) && pawn.CurJobDef == JobDefOf.LayDownAwake)
                {
                    pawn.jobs.StartJob(new Job(JobDefOf.LayDown, pawn.jobs.curJob.targetA), JobCondition.InterruptForced);

                    return true;
                }
                return false;
            }

            void newAct()
            {
                pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(BOT_ThoughtDefOf.BOT_NyctophobiaCantSleep);

                if (pawn.needs.rest.CurCategory == RestCategory.Exhausted) return;
                savingComp.NotifiedNyctoPawnSet.Remove(pawn);
            }

            __result.AddFailCondition(newFailCondition);
            __result.AddFinishAction(newAct);
        }
    }
}
