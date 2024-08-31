using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace More_Traits.ThoughtWorkers;

public class ThoughtWorker_AnimalWhispererAnimalCount : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn pawn)
    {
        if (!pawn.Spawned) return false;

        int score = Math.Min(10, Math.Max(pawn.Map.mapPawns.SpawnedColonyAnimals.Count, 0));
        return ThoughtState.ActiveAtStage(score);
    }
}
