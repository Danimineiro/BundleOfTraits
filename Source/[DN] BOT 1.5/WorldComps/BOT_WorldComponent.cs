using More_Traits.Extensions;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.WorldComps
{
    public class BOT_WorldComponent : WorldComponent
    {
        private bool postInitDone = false;

        private HashSet<Pawn> notifiedNyctoPawnSet = new HashSet<Pawn>();

        public HashSet<Pawn> NotifiedNyctoPawnSet => notifiedNyctoPawnSet;

        public BOT_WorldComponent(World world) : base(world) { }

        public static BOT_WorldComponent Instance => Verse.Current.Game.World.GetComponent<BOT_WorldComponent>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref notifiedNyctoPawnSet, nameof(notifiedNyctoPawnSet), LookMode.Reference);
        }

        public override void WorldComponentTick()
        {
            base.WorldComponentTick();

            if (!postInitDone)
            {
                foreach (Pawn pawn in PawnsFinder.All_AliveOrDead.Where(PawnExtensions.CanHandlePawn)) pawn.AddTraitHediffs();
                postInitDone = true;
            }
        }
    }
}
