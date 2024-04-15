using More_Traits.DefOfs;
using More_Traits.ModExtensions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace More_Traits.ThinkNodes
{
    public class BOT_JobGiverFleeing : ThinkNode_JobGiver
    {
        private static BOT_ThinkTreeExtension extension;

        public static BOT_ThinkTreeExtension Extension => extension ?? (extension = BOT_ThinkTreeDefOf.Bot_FleeingBehaviour.GetModExtension<BOT_ThinkTreeExtension>());

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.Drafted) return null;

            IEnumerable<TraitDef> pawnTraitDefs = pawn.story.traits.allTraits.Select(trait => trait.def);
            List<ThingDef> pawnDangerDefs = Extension.traitContainers.Where(container => pawnTraitDefs.Contains(container.traitDef)).Select(container => container.thingDef).ToList();

            if (pawnDangerDefs.Count == 0) return null;

            Map map = pawn.Map;
            ListerThings lister = map.listerThings;
            IEnumerable<Thing> livingDangers = map.mapPawns.AllPawnsSpawned.Where(spawned => pawnDangerDefs.Contains(spawned.def));
            List<Thing> objectDangers = ObjectDangers(lister, pawnDangerDefs, livingDangers, pawn, out Thing closest);

            if (objectDangers.Count == 0) return null;

            IntVec3 destination = CellFinderLoose.GetFleeDestToolUser(pawn, objectDangers);

            if (destination == pawn.Position) return null;

            return JobMaker.MakeJob(JobDefOf.Flee, destination, closest);
        }

        private List<Thing> ObjectDangers(ListerThings lister, List<ThingDef> pawnDangerDefs, IEnumerable<Thing> livingDangers, Pawn pawn, out Thing closestItem)
        {
            List<ThingDef> dangerDefsLivingRemoved = new List<ThingDef>(pawnDangerDefs.Count);
            List<Thing> result = new List<Thing>();
            float closestDistance = float.MaxValue;
            closestItem = null;


            foreach (Thing thing in livingDangers)
            {
                dangerDefsLivingRemoved.Add(thing.def);
                CheckDistances(pawn, ref closestItem, ref closestDistance, thing);

                result.Add(thing);
            }

            foreach (ThingDef dangerDef in pawnDangerDefs.Where(dangerDef => !dangerDefsLivingRemoved.Contains(dangerDef)))
            {
                foreach (Thing thing in lister.ThingsOfDef(dangerDef))
                {
                    CheckDistances(pawn, ref closestItem, ref closestDistance, thing);
                    result.Add(thing);
                }
            }

            return result;
        }

        private static void CheckDistances(Pawn pawn, ref Thing closestItem, ref float closestDistance, Thing thing)
        {
            float thingDistance = thing.Position.DistanceTo(pawn.Position);

            if (closestDistance > thingDistance)
            {
                closestDistance = thingDistance;
                closestItem = thing;
            }
        }
    }
}
