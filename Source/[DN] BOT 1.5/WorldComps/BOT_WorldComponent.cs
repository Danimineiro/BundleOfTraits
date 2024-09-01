using More_Traits.Extensions;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace More_Traits.WorldComps;

public class BOT_WorldComponent(World world) : WorldComponent(world)
{
    private bool postInitDone = false;

    private HashSet<Pawn> notifiedNyctoPawnSet = [];

    public HashSet<Pawn> NotifiedNyctoPawnSet => notifiedNyctoPawnSet;

    public static BOT_WorldComponent Instance => Current.Game.World.GetComponent<BOT_WorldComponent>();

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref notifiedNyctoPawnSet, nameof(notifiedNyctoPawnSet), LookMode.Reference);
    }

    public override void WorldComponentTick()
    {
        base.WorldComponentTick();

        if (postInitDone) return;
        

        Span<Pawn> values = PawnsFinder.All_AliveOrDead.AsSpanUnsafe();

        int count = values.Length;
        for (int i = 0; i < count; i++)
        {
            Pawn pawn = values[i];
            if (pawn.CanHandlePawn()) pawn.AddTraitHediffs();
        }

        postInitDone = true;
    }
}
