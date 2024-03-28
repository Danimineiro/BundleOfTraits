using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.WorldComps
{
    public class Saving : WorldComponent
    {
        public Saving(World world) : base(world) { }

        public static Saving Current => Verse.Current.Game.World.GetComponent<Saving>();

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
