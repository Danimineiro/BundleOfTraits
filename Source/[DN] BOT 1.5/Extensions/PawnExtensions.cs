using More_Traits.ModExtensions;
using RimWorld;
using Verse;

namespace More_Traits.Extensions
{
    public static class PawnExtensions
    {
        public static void TryGainMemory(this Pawn pawn, ThoughtDef thoughtDef, int forcedLevel)
        {
            pawn.needs.mood?.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(thoughtDef, forcedLevel));
        }

        public static void TryGainMemory(this Pawn pawn, ThoughtDef thoughtDef)
        {
            pawn.needs.mood?.thoughts.memories.TryGainMemory(thoughtDef);
        }

        public static bool HasTrait(this Pawn pawn, TraitDef traitDef)
        {
            return pawn.story?.traits.HasTrait(traitDef) == true;
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
    }
}
