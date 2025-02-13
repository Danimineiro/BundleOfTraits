using More_Traits.DefOfs;
using More_Traits.ModExtensions;
using Verse.AI;

namespace More_Traits.ThinkNodes;

public class BOT_JobGiverFleeing : ThinkNode_JobGiver
{
    private static Dictionary<TraitDef, TraitContainer>? traitContainers;
    private static BOT_ThinkTreeExtension? extension;
    const float MAX_DISTANCE = 20f;

    public static BOT_ThinkTreeExtension Extension => extension ??= BOT_ThinkTreeDefOf.Bot_FleeingBehaviour.GetModExtension<BOT_ThinkTreeExtension>();

    public static Dictionary<TraitDef, TraitContainer> TraitContainers => traitContainers ??= BuildTraitContainers();

    private static Dictionary<TraitDef, TraitContainer> BuildTraitContainers()
    {
        Dictionary<TraitDef, TraitContainer> containers = [];

        for (int i = 0; i < Extension.traitContainers.Count; i++)
        {
            TraitContainer container = Extension.traitContainers[i];
            containers.Add(container.traitDef, container);
        }

        return containers;
    }

    protected override Job? TryGiveJob(Pawn pawn)
    {
        if (!pawn.Spawned) return null;

        List<TraitDef> fleeTraits = GetTraits(pawn, out bool ignoreDrafted);
        int traitCount = fleeTraits.Count;

        if (traitCount == 0) return null;
        if (pawn.Drafted && !ignoreDrafted) return null;

        List<Thing> objectDangers = GetDangers(pawn, fleeTraits, out Thing? closest);
        if (objectDangers.Count == 0) return null;

        IntVec3 destination = CellFinderLoose.GetFleeDestToolUser(pawn, objectDangers);
        if (destination == pawn.Position) return null;

        Job job = JobMaker.MakeJob(JobDefOf.FleeAndCower, destination, closest);
        job.checkOverrideOnExpire = true;
        job.expiryInterval = 65;

        return job;
    }

    private List<Thing> GetDangers(Pawn pawn, List<TraitDef> fleeTraits, out Thing? closest)
    {
        Map map = pawn.Map;
        ListerThings lister = map.listerThings;

        IReadOnlyList<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
        HashSet<ThingDef> dangerDefs = [];
        HashSet<string> dangerNotes = [];

        int fleeTraitCount = fleeTraits.Count;
        for (int i = 0; i < fleeTraitCount; i++)
        {
            TraitDef traitDef = fleeTraits[i];
            TraitContainer container = TraitContainers[traitDef];

            foreach (ThingDef thingDef in container.thingDefs) dangerDefs.Add(thingDef);
            foreach (string devNote in container.devNotes) dangerNotes.Add(devNote);
        }

        List<Pawn> livingDangers = allPawnsSpawned.Where(spawned => dangerDefs.Contains(spawned.def) || spawned.def.devNote is string note && dangerNotes.Contains(note)).ToList();
        return ObjectDangers(lister, dangerDefs, livingDangers, pawn, out closest);
    }

    private List<TraitDef> GetTraits(Pawn pawn, out bool ignoreDrafted)
    {
        int count = pawn.story.traits.allTraits.Count;
        List<TraitDef> fleeTraits = [];
        ignoreDrafted = false;

        for (int i = 0; i < count; i++)
        {
            Trait trait = pawn.story.traits.allTraits[i];
            if (!TraitContainers.TryGetValue(trait.def, out TraitContainer traitContainer)) continue;

            fleeTraits.Add(traitContainer.traitDef);
            ignoreDrafted |= traitContainer.ignoreDraft;
        }

        return fleeTraits;
    }

    private List<Thing> ObjectDangers(ListerThings lister, HashSet<ThingDef> pawnDangerDefs, List<Pawn> livingDangers, Pawn pawn, out Thing? closestItem)
    {
        List<Thing> result = [];
        float closestDistance = float.MaxValue;
        closestItem = null;

        foreach (Thing thing in livingDangers)
        {
            pawnDangerDefs.Remove(thing.def);
            if (CheckDistances(pawn, ref closestItem, ref closestDistance, thing)) result.Add(thing);
        }

        foreach (ThingDef dangerDef in pawnDangerDefs)
        {
            foreach (Thing thing in lister.ThingsOfDef(dangerDef))
            {
                if (CheckDistances(pawn, ref closestItem, ref closestDistance, thing)) result.Add(thing);
            }
        }

        return result;
    }

    private static bool CheckDistances(Pawn pawn, ref Thing? closestItem, ref float closestDistance, Thing thing)
    {
        float thingDistance = thing.Position.DistanceTo(pawn.Position);

        if (closestDistance > thingDistance)
        {
            closestDistance = thingDistance;
            closestItem = thing;
        }

        return thingDistance <= MAX_DISTANCE && pawn.CanSee(thing);
    }
}
