using More_Traits.Extensions;
using UnityEngine;

namespace More_Traits.ThoughtWorkers;

internal class ThoughtWorker_FamilyOriented_ChildCount : ThoughtWorker
{
    private readonly static Dictionary<Map, (int childCount, int lastCountTick)> mapChildCount = [];

    /// <summary>
    ///     Quarter of a RimWorld day
    /// </summary>
    private const int CountTickInterval = 15000;

    public override float MoodMultiplier(Pawn pawn)
    {
        if (!pawn.Faction.IsPlayer) return 0f;
        if (!pawn.Spawned) return 0f;

        RefreshChildCountForMap(pawn.Map);

        return mapChildCount[pawn.Map].childCount switch
        {
            > 1 and int n => Mathf.Min(n * 3f, 30f),
            0 => 1f,
            _ => 0f
        };
    }

    protected override ThoughtState CurrentStateInternal(Pawn pawn)
    {
        if (!pawn.Faction.IsPlayer) return false;
        if (!pawn.Spawned) return false;

        RefreshChildCountForMap(pawn.Map);

        return mapChildCount[pawn.Map].childCount switch
        {
            > 1 => ThoughtState.ActiveAtStage(1),
            0 => ThoughtState.ActiveAtStage(0),
            _ => false
        };
    }

    private void RefreshChildCountForMap(Map map)
    {
        int currentTick = Find.TickManager.TicksGame;
        if (mapChildCount.TryGetValue(map, out (int childCount, int lastCountTick) value))
        {
            if (currentTick - CountTickInterval <= value.lastCountTick) return;
        }

        RefreshChildCount(map, currentTick);
    }

    private static void RefreshChildCount(Map map, int currentTick)
    {
        int childCount = map.mapPawns.FreeColonistsSpawned.UnsafeCount(pawn => !pawn.ageTracker.Adult);
        mapChildCount[map] = (childCount, currentTick);
    }
}
