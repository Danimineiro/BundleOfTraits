using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace More_Traits.ModExtensions
{
    public class Container
    {
        public TraitDef traitDef;
        public ThingDef thingDef;
    }

    public class BOT_ThinkTreeExtension : DefModExtension
    {
        public List<Container> traitContainers = new List<Container>();

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors()) yield return error;

            foreach (Container container in traitContainers)
            {
                if (container is null) yield return $"{nameof(container)} can't be null!";
            }
        }
    }
}
