using More_Traits.DefOfs;
using More_Traits.Extensions;

namespace More_Traits.HediffClass;

public sealed class BOT_AnimalWhispererInteractionHediff : Hediff
{
    private readonly static Dictionary<Pawn, int> bondedAnimals = [];

    private Dictionary<Pawn, InteractionCounter> interactionStages = [];

    private List<Pawn> keys = [];

    private List<InteractionCounter> values = [];

    public override void Tick()
    {
        if (!pawn.IsHashIntervalTick(400)) return;

        List<Pawn> keysToRemove = [];
        foreach((Pawn key, InteractionCounter counter) in interactionStages)
        {
            if (counter.interactions.Count == 0)
            {
                keysToRemove.Add(key);
                continue;
            }

            if (Find.TickManager.TicksGame - counter.interactions.Peek() <= 420_000) continue;
            
            counter.interactions.Dequeue();
        }

        for (int i = 0; i < keysToRemove.Count; i++)
        {
            interactionStages.Remove(keysToRemove[i]);
        }
    }

    public void AddInteraction(Pawn pawn)
    {
        if (!interactionStages.TryGetValue(pawn, out InteractionCounter counter))
        {
            counter = interactionStages[pawn] = new();
        }

        counter.interactions.Enqueue(Find.TickManager.TicksGame);
    }

    public static void AddInteraction(Pawn animal, Pawn human)
    {
        if (bondedAnimals.TryGetValue(animal, out int value) && value > 60_000 && animal.IsHashIntervalTick(1_000)) return;

        if (animal.relations.DirectRelations.UnsafeContains(relation => relation.def == PawnRelationDefOf.Bond))
        {
            bondedAnimals[animal] = Find.TickManager.TicksGame;
            return;
        }

        List<Hediff> hediffs = animal.health.hediffSet.hediffs;
        if (!hediffs.UnsafeTryGet(item => item.def == BOT_HediffDefOf.BOT_AnimalWhispererInteractionTag, out Hediff? hediff))
        {
            animal.health.AddHediff(BOT_HediffDefOf.BOT_AnimalWhispererInteractionTag);
            hediff = hediffs[hediffs.Count - 1];
        }

        ((BOT_AnimalWhispererInteractionHediff)hediff!).AddInteraction(human);
    }

    public override void ExposeData()
    {
        Scribe_Collections.Look(ref interactionStages, nameof(interactionStages), LookMode.Reference, LookMode.Deep, ref keys, ref values);
        base.ExposeData();
    }

    private record struct InteractionCounter() : IExposable
    {
        public Queue<int> interactions = [];

        public void ExposeData()
        {
            Scribe_Collections.Look(ref interactions, nameof(interactions));
        }
    }
}

