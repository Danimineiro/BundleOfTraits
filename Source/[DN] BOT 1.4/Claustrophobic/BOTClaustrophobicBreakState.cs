using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace More_Traits
{
	public class BOTClaustrophobicBreakState : MentalState
	{
        public new bool AllowRestingInBed => false;

        public override RandomSocialMode SocialModeMax() => RandomSocialMode.Off;

        public override void MentalStateTick()
        {
            if (pawn.CurJob?.def != JobDefOf.Wait_MaintainPosture)
            {
                pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, false, false);
                pawn.jobs.StartJob(new Job(JobDefOf.Wait_MaintainPosture));
            }
            base.MentalStateTick();
        }

        public override void PostEnd()
        {
            try
            {
                if (pawn.health.hediffSet.GetFirstHediffOfDef(BOTHediffDefOf.BOT_ClaustrophobicBreakdown) is Hediff hediff)
                    pawn.health.RemoveHediff(hediff);
                else
                {
                    Log.Error($"[Bundle of Traits] An error occured trying to remove the Claustrophobic panic attack hediff from Pawn <color=orange>{pawn.Name}</color>! The hediff was missing?");
                }
            }
            catch (Exception e)
            {
                Log.Error($"[Bundle of Traits] An error occured trying to remove the Claustrophobic panic attack hediff from Pawn <color=orange>{pawn.Name}</color>! Please check if the pawn still has the hediff and report this error: {e}");
            }
            base.PostEnd();
        }
    }
}
