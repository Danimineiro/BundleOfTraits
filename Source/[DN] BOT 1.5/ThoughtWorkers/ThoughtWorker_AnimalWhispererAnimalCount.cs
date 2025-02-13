namespace More_Traits.ThoughtWorkers;

public class ThoughtWorker_AnimalWhispererAnimalCount : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn pawn)
    {
        if (!pawn.Spawned) return false;

        int count = Math.Min(10, Math.Max(pawn.Map.mapPawns.SpawnedColonyAnimals.Count, 0));
        int score = count switch
        {
            >= 10 => 4,
            >= 5 => 3,
            >= 2 => 2,
            >= 1 => 1,
            _ => 0
        };

        return ThoughtState.ActiveAtStage(score);
    }
}
