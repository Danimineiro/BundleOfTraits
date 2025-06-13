using More_Traits.ModExtensions;

namespace More_Traits.Extensions;

public static class PawnExtensions
{
    public static void TryGainMemory(this Pawn pawn, ThoughtDef thoughtDef, int forcedLevel, Precept? source = null)
    {
        pawn.needs.mood?.thoughts.memories.TryGainMemoryFast(thoughtDef, forcedLevel, source);
    }

    public static void TryGainMemory(this Pawn pawn, ThoughtDef thoughtDef, Pawn? other = null, Precept? source = null)
    {
        pawn.needs.mood?.thoughts.memories.TryGainMemory(thoughtDef, other, source);
    }

    public static bool HasTrait(this Pawn pawn, TraitDef traitDef)
    {
        return pawn.story?.traits.allTraits.UnsafeContains(trait => trait.def == traitDef) ?? false;
    }

    public static bool CanHandlePawn(this Pawn pawn) => !pawn.Dead && pawn.story?.traits != null;

    public static void AddTraitHediffs(this Pawn pawn)
    {
        foreach (Trait trait in pawn.story.traits.allTraits)
        {
            if (trait.def.GetModExtension<BOT_TraitExtension>() is not BOT_TraitExtension link) continue;
            pawn.health.GetOrAddHediff(link.hediffDef);
        }
    }

    public static bool HasRelationDef(this Pawn pawn, PawnRelationDef relationDef) => TryGetRelation(pawn, relationDef, out _);

    public static bool TryGetRelation(this Pawn pawn, PawnRelationDef relationDef, out DirectPawnRelation? directPawnRelation, Pawn? otherPawn = null)
    {
        ReadOnlySpan<DirectPawnRelation> relations = pawn.relations.DirectRelations.AsSpanUnsafe();

        int count = relations.Length;
        bool otherPawnNotNull = otherPawn != null;
        for (int i = 0; i < count; i++)
        {
            DirectPawnRelation relation = relations[i];
            if (relation.def != relationDef && otherPawnNotNull && relation.otherPawn != otherPawn) continue;

            directPawnRelation = relation;
            return true;
        }

        directPawnRelation = null;
        return false;
    }
}
