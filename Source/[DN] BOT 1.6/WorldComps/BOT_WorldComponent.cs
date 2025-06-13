using HarmonyLib;
using More_Traits.Extensions;
using RimWorld.Planet;

namespace More_Traits.WorldComps;

public class BOT_WorldComponent(World world) : WorldComponent(world)
{
    private HashSet<Pawn> notifiedNyctoPawnSet = [];

    public HashSet<Pawn> NotifiedNyctoPawnSet => notifiedNyctoPawnSet;

    public static BOT_WorldComponent Instance => Current.Game.World.GetComponent<BOT_WorldComponent>();

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref notifiedNyctoPawnSet, nameof(notifiedNyctoPawnSet), LookMode.Reference);
    }

    /// <summary>
    ///     Adds <see cref="Trait"/> related <see cref="Hediff"/>s on world start.
    /// </summary>
    /// <param name="fromLoad">Ignored.</param>
    public override void FinalizeInit(bool fromLoad)
    {
        PawnsFinder.All_AliveOrDead
            .Where(pawn => pawn.CanHandlePawn())
            .Do(pawn => pawn.AddTraitHediffs());

        base.FinalizeInit(fromLoad);
    }
}
